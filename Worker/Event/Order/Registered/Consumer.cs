using Common.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Refit;

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
                var inventoryService = RestService.For<IInventoryApi>(apisConfig.Value.InventoryApi.BaseUrl);
                var orderService = RestService.For<IOrderApi>(apisConfig.Value.OrderApi.BaseUrl);

                foreach (var item in message.Items)
                {
                    var request = new UpdateStockHistoryRequest
                    {
                        OperationType = 2,
                        Quantity = 1
                    };
                    
                    var response = await inventoryService.UpdateStockHistoryAsync(item, request);
                    
                    var itemStatus = 2;

                    if (response is null)
                    {
                        itemStatus = 3;
                    }
                        
                    await orderService.UpdateOrderStatusAsync(message.OrderID, new UpdateOrderStatusRequest
                    {
                        ItemId = item,
                        Status = itemStatus
                    });
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