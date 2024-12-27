using System.Text.Json;
using Common.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Worker.Event.Product.StockUpdated;

internal sealed class Consumer(IOptions<ApisConfig> apisConfig, IConsumer<string, Message> consumer) : IHostedService
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        consumer.Subscribe(Constants.ProductStockUpdatedTopic);

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(_cancellationTokenSource.Token);

            if (consumeResult.Message is not null)
            {
                var message = consumeResult.Message.Value;

                if (message.OperationType == 1)
                {
                    var orderRestClient = new RestClient(apisConfig.Value.OrderApi.BaseUrl);
                    var orderRequest = new RestRequest($"orders/status/3/products/{message.ProductID}");
                    var orderResponse = await orderRestClient.ExecuteAsync(orderRequest, _cancellationTokenSource.Token);

                    if (orderResponse.IsSuccessStatusCode)
                    {
                        var orders = JsonSerializer.Deserialize<Response[]>(orderResponse.Content);
                        var inventoryRestClient = new RestClient(apisConfig.Value.InventoryApi.BaseUrl);

                        foreach (var order in orders)
                        {
                            var itemsCount = order.Items.Count(i => i == message.ProductID);
                            var inventoryRequest = new RestRequest($"/products/{message.ProductID}/stock-history", Method.Put);

                            inventoryRequest.AddJsonBody(new
                            {
                                OperationType = 2,
                                Quantity = itemsCount
                            });

                            var inventoryResponse = await inventoryRestClient.ExecuteAsync(inventoryRequest, _cancellationTokenSource.Token);

                            if (inventoryResponse.IsSuccessStatusCode)
                            {
                                await UpdateOrderStatusAsync(order.Id, 2, message.ProductID);
                            }
                            else
                            {
                                await UpdateOrderStatusAsync(order.Id, 3, message.ProductID);
                            }
                        }
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
    public static void AddProductStockUpdatedConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IConsumer<string, Message>>(sp =>
            new ConsumerBuilder<string, Message>(
                    configuration.GetSection("Kafka:ConsumerConfig").Get<ConsumerConfig>())
                .SetValueDeserializer(new CustomJsonSerializer<Message>())
                .Build());

        services.AddHostedService<Consumer>();
    }
}