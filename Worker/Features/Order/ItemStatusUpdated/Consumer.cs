using Common.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Refit;
using Worker.Events.Order.ItemStatusUpdated;

namespace Worker.Features.Order.ItemStatusUpdated;

internal sealed class Consumer(
    IOptions<ApisConfig> apisConfig,
    IOptions<NotificationConfig> notificationConfig,
    IConsumer<string, Message> consumer) : IHostedService
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        consumer.Subscribe(Constants.OrderItemStatusUpdated);

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(_cancellationTokenSource.Token);

            if (consumeResult.Message is not null)
            {
                var message = consumeResult.Message.Value;

                if (message.Status == 3)
                {
                    var service = RestService.For<INotificationApi>(apisConfig.Value.NotificationApi.BaseUrl);
                    
                    await service.SendOutOfStockNotificationAsync(message.ItemId, new SendOutOfStockNotificationRequest
                    {
                        To = notificationConfig.Value.StockManager
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
    public static void AddOrderItemStatusUpdatedConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IConsumer<string, Message>>(sp =>
            new ConsumerBuilder<string, Message>(
                    configuration.GetSection("Kafka:Consumer").Get<ConsumerConfig>())
                .SetValueDeserializer(new CustomJsonSerializer<Message>())
                .Build());

        services.AddHostedService<Consumer>();
    }
}