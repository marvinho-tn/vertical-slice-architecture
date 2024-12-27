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
                var separatedProducts = 0;
                var restClient = new RestClient(apisConfig.Value.InventoryApi.BaseUrl);

                foreach (var item in consumeResult.Message.Value.Items)
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
                        separatedProducts++;
                    }
                    else
                    {
                        await UpdateOrderStatusAsync(consumeResult, 3);
                    }
                }

                if (separatedProducts == consumeResult.Message.Value.Items.Length)
                {
                    await UpdateOrderStatusAsync(consumeResult, 2);
                }

                consumer.Commit(consumeResult);
            }
        }
    }

    private async Task UpdateOrderStatusAsync(ConsumeResult<string, Message> consumeResult, int orderStatus)
    {
        var restClient = new RestClient(apisConfig.Value.OrderApi.BaseUrl);
        var request = new RestRequest($"/orders/{consumeResult.Message.Value.OrderID}/status", Method.Put);

        request.AddJsonBody(new
        {
            Id = consumeResult.Message.Value.OrderID,
            Status = orderStatus,
        });

        var response = await restClient.ExecuteAsync(request, _cancellationTokenSource.Token);

        if (response.IsSuccessStatusCode)
        {
            consumer.Commit(consumeResult);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        consumer.Close();
        consumer.Dispose();

        return Task.CompletedTask;
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