using Confluent.Kafka;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Worker.Event.OrderRegistered;

internal sealed class OrderRegisteredConsumer
(
    IOptions<ApisConfig> apisConfig,
    IProducer<string, ProductOutOfStockEvent> productOutOfStockProducer,
    IProducer<string, OrderSeparatedEvent> orderSeparatedProducer,
    IConsumer<string, OrderRegisteredEvent> consumer
)
    : IHostedService
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
                var separatedProducts = 0;
                var restClient = new RestClient(apisConfig.Value.InventoryApi.BaseUrl);
                
                foreach (var item in consumeResult.Message.Value.Items)
                {
                    var request = new RestRequest($"/products/{item}/stock-history", Method.Put);

                    request.AddJsonBody(new UpdateProductStockRequest
                    {
                        OperationType = 2,
                        Quantity = 1
                    });

                    var response = await restClient.ExecuteAsync(request, _cancellationTokenSource.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        separatedProducts++;
                    }
                    else
                    {
                        await productOutOfStockProducer.ProduceAsync(Constants.ProductOutOfStockTopic,
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
                }

                if (separatedProducts == consumeResult.Message.Value.Items.Length)
                {
                    await orderSeparatedProducer.ProduceAsync(Constants.OrderSeparatedTopic,
                        new Message<string, OrderSeparatedEvent>
                        {
                            Key = Guid.NewGuid().ToString(),
                            Value = new OrderSeparatedEvent
                            {
                                OrderID = consumeResult.Message.Value.OrderID
                            }
                        }, _cancellationTokenSource.Token);
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