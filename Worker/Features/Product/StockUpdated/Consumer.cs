using Common.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Worker.Events.Product.StockUpdated;
using Worker.Http;

namespace Worker.Features.Product.StockUpdated;

internal sealed class Consumer(
    IOptions<ApisConfig> apisConfig,
    IOptions<ConsumerConfig> consumerConfig,
    ILogger<Consumer> logger) : BackgroundService
{
    private readonly IConsumer<string, Message> _consumer = new ConsumerBuilder<string, Message>(consumerConfig.Value)
        .SetValueDeserializer(new CustomJsonSerializer<Message>())
        .Build();

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _consumer.Subscribe(Constants.ProductStockUpdatedTopicName);

        while (!ct.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(ct);

            logger.LogInformation($"{nameof(Constants.ProductStockUpdatedTopicName)} consumer started");

            if (consumeResult.Message is not null)
            {
                logger.LogInformation("Consumed message {0} from {1}", consumeResult.Message.Key,
                    Constants.ProductStockUpdatedTopicName);

                var message = consumeResult.Message.Value;

                if (message.OperationType == 1)
                {
                    var orderService = HttpExtensions.CreateHttpService<IOrderApi>(apisConfig.Value.OrderApi.BaseUrl);
                    var orders = await orderService.GetOrdersByStatusAndProductIdAsync(3, message.ProductID);
                    var inventoryService = HttpExtensions.CreateHttpService<IInventoryApi>(apisConfig.Value.InventoryApi.BaseUrl);

                    foreach (var order in orders)
                    {
                        var itemsCount = order.Items.Count(i => i == message.ProductID);
                        var updateStockHistoryRequest = new UpdateStockHistoryRequest
                        {
                            OperationType = 2,
                            Quantity = itemsCount
                        };
                        var updateStockHistoryResponse = await inventoryService.UpdateStockHistoryAsync(message.ProductID, updateStockHistoryRequest);
                        var itemStatus = 2;

                        if (updateStockHistoryResponse is null)
                        {
                            itemStatus = 3;
                        }

                        var updateOrderStatusRequest = new UpdateOrderStatusRequest
                        {
                            ItemId = message.ProductID,
                            Status = itemStatus
                        };
                        
                        await orderService.UpdateOrderStatusAsync(order.Id, updateOrderStatusRequest);
                    }
                }

                _consumer.Commit(consumeResult);
            }
        }
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
    }
}

internal static class DependencyConfiguration
{
    public static void AddProductStockUpdatedConsumer(this IServiceCollection services)
    {
        services.AddHostedService<Consumer>();
    }
}