using System.Text.Json;
using Common.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Worker.Event.Order.Separated;

internal sealed class Consumer(IConsumer<string, Message> consumer, IOptions<ApisConfig> apisConfig) : IHostedService
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        consumer.Subscribe(Constants.OrderSeparatedTopic);

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(_cancellationTokenSource.Token);

            if (consumeResult.Message is not null)
            {
                var restClient = new RestClient(apisConfig.Value.OrderApi.BaseUrl);
                var getOrderRequest = new RestRequest($"/orders/{consumeResult.Message.Value.OrderID}", Method.Get);
                var getOrderResponse = await restClient.ExecuteAsync(getOrderRequest, _cancellationTokenSource.Token);

                if (getOrderResponse.IsSuccessStatusCode)
                {
                    var order = JsonSerializer.Deserialize<Response>(getOrderResponse.Content);
                    var updateOrderRequest =
                        new RestRequest($"/orders/{order.Id}", Method.Put);

                    updateOrderRequest.AddJsonBody(new Request
                    {
                        Id = order.Id,
                        Client = order.Client,
                        Status = 2,
                        Items = order.Items
                    });

                    var updateOrderResponse =
                        await restClient.ExecuteAsync(updateOrderRequest, _cancellationTokenSource.Token);

                    if (updateOrderResponse.IsSuccessStatusCode)
                    {
                        consumer.Commit(consumeResult);
                    }
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        consumer.Close();
        consumer.Dispose();
        
        return Task.CompletedTask;
    }
}

internal static class Configuration
{
    public static void AddOrderSeparatedEvent(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IConsumer<string, Message>>(sp =>
            new ConsumerBuilder<string, Message>(
                    configuration.GetSection("Kafka:ConsumerConfig").Get<ConsumerConfig>())
                .SetValueDeserializer(new CustomJsonSerializer<Message>())
                .Build());

        services.AddHostedService<Consumer>();
    }
}