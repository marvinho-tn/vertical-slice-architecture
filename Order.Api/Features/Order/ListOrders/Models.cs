using FastEndpoints;
using FluentValidation;

namespace Order.Api.Features.Order.ListOrders
{
    internal sealed class Request
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    internal sealed class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Página deve ser maior ou igual a 1");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Tamanho da página deve estar entre 1 e 100");
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
