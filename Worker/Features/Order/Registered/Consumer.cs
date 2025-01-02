using Common.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Worker.Events.Order.Registered;
using Worker.Http;

namespace Worker.Features.Order.Registered;

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
        _consumer.Subscribe(Constants.OrderRegisteredTopicName);

        while (!ct.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(ct);

            logger.LogInformation($"{nameof(Constants.OrderRegisteredTopicName)} consumer started");

            if (consumeResult.Message is not null)
            {
                logger.LogInformation("Consumed message {0} from {1}", consumeResult.Message.Key,
                    Constants.OrderRegisteredTopicName);

                var message = consumeResult.Message.Value;
                var inventoryService = HttpExtensions.CreateHttpService<IInventoryApi>(apisConfig.Value.InventoryApi.BaseUrl);
                var orderService = HttpExtensions.CreateHttpService<IOrderApi>(apisConfig.Value.OrderApi.BaseUrl);

                foreach (var item in message.Items)
                {
                    var updateStockHistoryRequest = new UpdateStockHistoryRequest
                    {
                        OperationType = 2,
                        Quantity = 1
                    };

                    var updateStockHistoryResponse = await inventoryService.UpdateStockHistoryAsync(item, updateStockHistoryRequest);

                    var itemStatus = 2;

                    if (updateStockHistoryResponse is null)
                    {
                        itemStatus = 3;
                    }

                    var updateOrderStatusRequest = new UpdateOrderStatusRequest
                    {
                        ItemId = item,
                        Status = itemStatus
                    };
                    
                    await orderService.UpdateOrderStatusAsync(message.OrderID, updateOrderStatusRequest);
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
    public static void AddOrderRegisteredConsumer(this IServiceCollection services)
    {
        services.AddHostedService<Consumer>();
    }
}