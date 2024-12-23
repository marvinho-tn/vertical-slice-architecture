using Common.Data;
using FastEndpoints;
using FluentValidation;

namespace Order.Api.Features.Order.CreateOrder
{
    internal sealed class Request
    {
        public string Client { get; set; }
        public string[] Items { get; set; }
    }

    internal sealed class Validator : Validator<Request>
    {
        public Validator(MongoDbContext dbContext)
        {
            RuleFor(x => x.Client)
                .NotNull()
                .WithMessage("Cliente do pedido é obrigatório");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Pedido deve conter ao menos um item");

            RuleForEach(x => x.Items)
                .NotEmpty()
                .WithMessage("Nome do item do pedido é obrigatório")
                .Must((request, item, cancellation) => dbContext.Exists<ProductEntity>(Constants.ProductsCollectionName, item))
                .WithMessage("Item não existe no cadastro de produtos");
        }
    }

    internal sealed class Response
    {
        public string Id { get; set; }
        public string Client { get; set; }
        public int Status { get; set; }
        public string[] Items { get; set; }
    }
}