using Common.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Refit;
using Worker.Events.Product.StockUpdated;

namespace Worker.Features.Product.StockUpdated;

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
                    var orderService = RestService.For<IOrderApi>(apisConfig.Value.OrderApi.BaseUrl);
                    var orders = await orderService.GetOrdersByStatusAndProductIdAsync(3, message.ProductID);
                    var inventoryService = RestService.For<IInventoryApi>(apisConfig.Value.InventoryApi.BaseUrl);

                    foreach (var order in orders)
                    {
                        var itemsCount = order.Items.Count(i => i == message.ProductID);
                        var request = new UpdateStockHistoryRequest
                        {
                            OperationType = 2,
                            Quantity = itemsCount
                        };
                        var response = await inventoryService.UpdateStockHistoryAsync(message.ProductID, request);
                        var itemStatus = 2;

                        if (response is null)
                        {
                            itemStatus = 3;
                        }
                        
                        await orderService.UpdateOrderStatusAsync(order.Id, new UpdateOrderStatusRequest
                        {
                            ItemId = message.ProductID,
                            Status = itemStatus
                        });
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
}

internal static class DependencyConfiguration
{
    public static void AddProductStockUpdatedConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IConsumer<string, Message>>(sp =>
            new ConsumerBuilder<string, Message>(
                    configuration.GetSection("Kafka:Consumer").Get<ConsumerConfig>())
                .SetValueDeserializer(new CustomJsonSerializer<Message>())
                .Build());

        services.AddHostedService<Consumer>();
    }
}