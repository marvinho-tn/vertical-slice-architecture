using Common.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Worker.Events.Order.ItemStatusUpdated;
using Worker.Http;

namespace Worker.Features.Order.ItemStatusUpdated;

internal sealed class Consumer(
    IOptions<ApisConfig> apisConfig,
    IOptions<ConsumerConfig> consumerConfig,
    IOptions<NotificationConfig> notificationConfig,
    ILogger<Consumer> logger) : BackgroundService
{
    private readonly IConsumer<string, Message> _consumer = new ConsumerBuilder<string, Message>(consumerConfig.Value)
        .SetValueDeserializer(new CustomJsonSerializer<Message>())
        .Build();
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _consumer.Subscribe(Constants.OrderItemStatusUpdatedTopicName);

        while (!ct.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(ct);
            
            logger.LogInformation($"{nameof(Constants.OrderItemStatusUpdatedTopicName)} consumer started");

            if (consumeResult.Message is not null)
            {
                logger.LogInformation("Consumed message {0} from {1}", consumeResult.Message.Key, Constants.OrderItemStatusUpdatedTopicName);
                
                var message = consumeResult.Message.Value;

                if (message.Status == 3)
                {
                    var service = HttpExtensions.CreateHttpService<INotificationApi>(apisConfig.Value.NotificationApi.BaseUrl);
                    
                    await service.SendOutOfStockNotificationAsync(message.ItemId, new SendOutOfStockNotificationRequest
                    {
                        To = notificationConfig.Value.StockManager
                    });
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
    public static void AddOrderItemStatusUpdatedConsumer(this IServiceCollection services)
    {
        services.AddHostedService<Consumer>();
    }
}