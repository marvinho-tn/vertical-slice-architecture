using Common.Data;
using Common.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Inventory.Api.Features.Product.ControlStockHistory;

internal sealed class OrderRegisteredMessage
{
    public string OrderID { get; set; }
    public string[] Items { get; set; }
}

internal sealed class ProductOutOfStockEvent
{
    public string SourceOrderID { get; set; }
    public string ProductID { get; set; }
}

internal sealed class OrderSeparatedEvent
{
    public string OrderID { get; set; }
}

internal sealed class OrderRegisteredConsumer : IHostedService
{
    private readonly IDbContext _dbContext;
    private readonly IProducer<string, ProductOutOfStockEvent> _productOutOfStockProducer;
    private readonly IProducer<string, OrderSeparatedEvent> _orderSeparatedProducer;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IConsumer<string, OrderRegisteredMessage> _consumer;

    public OrderRegisteredConsumer
    (
        IOptions<ConsumerConfig> config,
        IDbContext dbContext, 
        IProducer<string, ProductOutOfStockEvent> productOutOfStockProducer,
        IProducer<string, OrderSeparatedEvent> orderSeparatedProducer
    )
    {
        _dbContext = dbContext;
        _productOutOfStockProducer = productOutOfStockProducer;
        _orderSeparatedProducer = orderSeparatedProducer;
        _consumer = new ConsumerBuilder<string, OrderRegisteredMessage>(config.Value)
            .SetValueDeserializer(new CustomJsonSerializer<OrderRegisteredMessage>())
            .Build();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(Constants.OrderRegisteredTopic);

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(_cancellationTokenSource.Token);

            if (consumeResult.Message is not null)
            {
                var separatedItems = 0;
                
                foreach (var item in consumeResult.Message.Value.Items)
                {
                    var product = _dbContext.GetById<ProductEntity>(item);

                    if (product is null || product.QuantityInStock == 0)
                    {
                        await _productOutOfStockProducer.ProduceAsync(Constants.ProductOutOfStockTopic,
                            new Message<string, ProductOutOfStockEvent>
                            {
                                Key = Guid.NewGuid().ToString(),
                                Value = new ProductOutOfStockEvent
                                {
                                    SourceOrderID = consumeResult.Message.Value.OrderID,
                                    ProductID = item
                                }
                            }, _cancellationTokenSource.Token);
                    }
                    else
                    {
                        product.QuantityInStock--;
                        product.ProductStockHistory.Add(new ProductEntity.ProductStockEntity
                        {
                            Operation = ProductEntity.ProductStockEntity.StockOperationType.Decrease,
                            Quantity = 1
                        });

                        _dbContext.Update(product.Id, product);

                        separatedItems++;
                    }
                }

                if (separatedItems == consumeResult.Value.Items.Length)
                {
                    await _orderSeparatedProducer.ProduceAsync(Constants.OrderSeparatedTopic,
                        new Message<string, OrderSeparatedEvent>
                        {
                            Key = Guid.NewGuid().ToString(),
                            Value = new OrderSeparatedEvent
                            {
                                OrderID = consumeResult.Message.Value.OrderID
                            }
                        }, _cancellationTokenSource.Token);
                }

                _consumer.Commit(consumeResult);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();

        return Task.CompletedTask;
    }
}