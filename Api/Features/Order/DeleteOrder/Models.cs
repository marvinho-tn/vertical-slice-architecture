using FastEndpoints;
using FluentValidation;

namespace Api.Features.Order.DeleteOrder
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
                .WithMessage("Id do pedido é obrigatório");
        }
    }
}
