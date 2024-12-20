using FastEndpoints;
using FluentValidation;

namespace Inventory.Api.Features.Product.RegisterProduct
{
    internal sealed class Request
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int QuantityInStock { get; set; }
    }

    internal sealed class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nome do produto é obrigatório");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Descrição do produto é obrigatória");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Preço do produto deve ser maior que zero");
        }
    }

    internal sealed class Response
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int QuantityInStock { get; set; }
    }   
}
