using Common.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Worker.Event.Order.ItemStatusUpdated;

internal sealed class Consumer(IOptions<ApisConfig> apisConfig, IConsumer<string, Registered.Message> consumer) : IHostedService
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
                // Send email to update stock
                
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
        services.AddTransient<IConsumer<string, Registered.Message>>(sp =>
            new ConsumerBuilder<string, Registered.Message>(
                    configuration.GetSection("Kafka:ConsumerConfig").Get<ConsumerConfig>())
                .SetValueDeserializer(new CustomJsonSerializer<Registered.Message>())
                .Build());

        services.AddHostedService<Consumer>();
    }
}