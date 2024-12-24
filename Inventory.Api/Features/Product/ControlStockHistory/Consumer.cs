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

internal sealed class OrderRegisteredConsumer : IHostedService
{
    private readonly IDbContext _dbContext;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IConsumer<string, OrderRegisteredMessage> _consumer;

    public OrderRegisteredConsumer(IOptions<ConsumerConfig> config, IDbContext dbContext)
    {
        _dbContext = dbContext;
        _consumer = new ConsumerBuilder<string, OrderRegisteredMessage>(config.Value)
            .SetValueDeserializer(new CustomJsonSerializer<OrderRegisteredMessage>())
            .Build();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(Constants.OrderRegisteredTopic);
        
        Task.Run(() =>
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(_cancellationTokenSource.Token);

                if (consumeResult.Message is not null)
                {
                    foreach (var item in consumeResult.Message.Value.Items)
                    {
                        var product = _dbContext.GetById<ProductEntity>(item);
                        
                        if(product is null || product.QuantityInStock == 0)
                        {
                            //Public a message to a topic to notify that the product is out of stock
                        }
                        else
                        {
                            product.QuantityInStock--;
                            
                            _dbContext.Update(product.Id, product);
                            
                            //Public a message to a topic to notify that the product stock has been updated
                        }
                    }
                    
                    _consumer.Commit(consumeResult);
                }
            }
        });
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();

        return Task.CompletedTask;
    }
}