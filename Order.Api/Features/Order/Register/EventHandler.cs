using Common.Serialization;
using Confluent.Kafka;
using FastEndpoints;

namespace Order.Api.Features.Order.Register;

internal sealed class Event
{
    public string OrderID { get; set; }
    public string[] Items { get; set; }
}

internal sealed class EventHandler(IProducer<string, Event> producer, ILogger<EventHandler> logger) : IEventHandler<Event>
{
    public async Task HandleAsync(Event eventModel, CancellationToken ct)
    {
        logger.LogInformation("Handling event for order registered {OrderID}", eventModel.OrderID);
        
        var message = new Message<string, Event>
        {
            Key = Guid.NewGuid().ToString(),
            Value = eventModel
        };
        
        await producer.ProduceAsync(Constants.OrderRegisteredTopic, message, ct);
    }
}

internal static class DependencyConfiguration
{
    public static IServiceCollection AddOrderRegisteredEvent(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddTransient<IProducer<string, Event>>(sp =>
            new ProducerBuilder<string, Event>(
                    configuration.GetSection("Kafka:Producer").Get<ProducerConfig>())
                .SetValueSerializer(new CustomJsonSerializer<Event>())
                .Build());
    }
}