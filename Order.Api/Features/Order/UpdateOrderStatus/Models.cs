using Common.Data;
using FastEndpoints;
using FluentValidation;

namespace Order.Api.Features.Order.UpdateOrderStatus
{
    internal sealed class Request
    {
        public string Id { get; set; }
        public int Status { get; set; }
    }

    internal sealed class Validator : Validator<Request>
    {
        public Validator(IDbContext dbContext)
        {
            RuleFor(x => x.Id)
                .NotNull()
                .WithMessage("Id do pedido é obrigatório");

            RuleFor(x => x.Status)
                .NotNull()
                .WithMessage("Status do pedido é obrigatório");
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