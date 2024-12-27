using Common.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Worker.Event.Order.Registered;

internal sealed class Consumer(IOptions<ApisConfig> apisConfig, IConsumer<string, Message> consumer) : IHostedService
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        consumer.Subscribe(Constants.OrderRegisteredTopic);

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(_cancellationTokenSource.Token);

            if (consumeResult.Message is not null)
            {
                var message = consumeResult.Message.Value;
                var restClient = new RestClient(apisConfig.Value.InventoryApi.BaseUrl);

                foreach (var item in message.Items)
                {
                    var request = new RestRequest($"/products/{item}/stock-history", Method.Put);

                    request.AddJsonBody(new
                    {
                        OperationType = 2,
                        Quantity = 1
                    });

                    var response = await restClient.ExecuteAsync(request, _cancellationTokenSource.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        await UpdateOrderStatusAsync(message.OrderID, 2, item);
                    }
                    else
                    {
                        await UpdateOrderStatusAsync(message.OrderID, 3, item);
                    }
                }

                consumer.Commit(consumeResult);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        consumer.Close();
        consumer.Dispose();

        return Task.CompletedTask;
    }

    private async Task UpdateOrderStatusAsync(string orderId, int orderStatus, string itemId)
    {
        var restClient = new RestClient(apisConfig.Value.OrderApi.BaseUrl);
        var request = new RestRequest($"/orders/{orderId}/status", Method.Put);

        request.AddJsonBody(new
        {
            Id = orderId,
            ItemId = itemId,
            Status = orderStatus,
        });

        await restClient.ExecuteAsync(request, _cancellationTokenSource.Token);
    }
}

internal static class DependencyConfiguration
{
    public static void AddOrderRegisteredConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IConsumer<string, Message>>(sp =>
            new ConsumerBuilder<string, Message>(
                    configuration.GetSection("Kafka:ConsumerConfig").Get<ConsumerConfig>())
                .SetValueDeserializer(new CustomJsonSerializer<Message>())
                .Build());

        services.AddHostedService<Consumer>();
    }
}