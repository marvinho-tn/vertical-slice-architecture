using Confluent.Kafka;
using FastEndpoints;

namespace Order.Api.Features.Order.CreateOrder;

internal sealed class OrderRegisteredEvent
{
    public string OrderID { get; set; }
    public string[] Items { get; set; }
}

internal sealed class OrderRegisteredEventHandler(IProducer<string, OrderRegisteredEvent> producer)
    : IEventHandler<OrderRegisteredEvent>
{
    public async Task HandleAsync(OrderRegisteredEvent eventModel, CancellationToken ct)
    {
        var message = new Message<string, OrderRegisteredEvent>
        {
            Key = Guid.NewGuid().ToString(),
            Value = eventModel
        };
        
        await producer.ProduceAsync(Constants.OrderRegisteredTopic, message, ct);
    }
}