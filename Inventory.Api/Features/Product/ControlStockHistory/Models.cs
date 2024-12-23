using Common.Data;
using FastEndpoints;
using FluentValidation;

namespace Inventory.Api.Features.Product.ControlStockHistory
{
    internal sealed class Request
    {
        public string Id { get; set; }
        public ProductEntity.ProductStockEntity.StockOperationType OperationType { get; set; }
        public int Quantity { get; set; }
    }

    internal sealed class Validator : Validator<Request>
    {
        public Validator(MongoDbContext dbContext)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id do produto é obrigatório");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantidade deve ser maior que zero");

            RuleFor(x => x.OperationType)
                .IsInEnum()
                .WithMessage("Tipo de operação inválido")
                .Must((request, cancellation) =>
                {
                    if (request.OperationType == ProductEntity.ProductStockEntity.StockOperationType.Decrease)
                    {
                        var product = dbContext.GetById<ProductEntity>(Constants.ProductsCollectionName, request.Id);

                        return product is not null && product.QuantityInStock >= request.Quantity;
                    }

                    return true;
                })
                .WithMessage("Quantidade em estoque insuficiente para a operação de diminuição");
        }
    }

    internal sealed class Response
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int QuantityInStock { get; set; }
    }
}
