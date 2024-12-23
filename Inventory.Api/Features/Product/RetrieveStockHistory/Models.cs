using FastEndpoints;
using FluentValidation;

namespace Inventory.Api.Features.Product.RetrieveStockHistory
{
    internal sealed class Request
    {
        public string Id { get; set; }
    }

    internal sealed class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id do produto é obrigatório");
        }
    }

    internal sealed class Response
    {
        public int Quantity { get; set; }
        public int Operation { get; set; }
    }
}
