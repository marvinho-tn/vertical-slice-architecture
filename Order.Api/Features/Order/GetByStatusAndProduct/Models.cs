using FastEndpoints;
using FluentValidation;

namespace Order.Api.Features.Order.GetByStatusAndProduct;

internal sealed class Request
{
    public string Id { get; set; }
    public int Status { get; set; }
}

internal sealed class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id do produto é obrigatório");
        
        RuleFor(x => x.Status)
            .NotEmpty()
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