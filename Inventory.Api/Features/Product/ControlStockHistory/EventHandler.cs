using Common.Serialization;
using Confluent.Kafka;
using FastEndpoints;

namespace Inventory.Api.Features.Product.ControlStockHistory;

internal sealed class Event
{
    public string ProductId { get; set; }
}

internal sealed class EventHandler(IProducer<string, Event> producer) : IEventHandler<Event>
{
    public async Task HandleAsync(Event eventModel, CancellationToken ct)
    {
        var message = new Message<string, Event>
        {
            Key = Guid.NewGuid().ToString(),
            Value = eventModel
        };
        
        await producer.ProduceAsync(Constants.ProductStockUpdatedTopicName, message, ct);
    }
}

internal static class DependencyConfiguration
{
    public static IServiceCollection AddProductStockUpdatedEvent(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddTransient<IProducer<string, Event>>(sp =>
            new ProducerBuilder<string, Event>(
                    configuration.GetSection("Kafka:ProducerConfig").Get<ProducerConfig>())
                .SetValueSerializer(new CustomJsonSerializer<Event>())
                .Build());
    }
}