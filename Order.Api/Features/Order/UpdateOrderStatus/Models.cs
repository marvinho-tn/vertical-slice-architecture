using Common.Data;
using FastEndpoints;
using FluentValidation;

namespace Order.Api.Features.Order.UpdateOrderStatus
{
    internal sealed class Request
    {
        public string Id { get; set; }
        public string ItemId { get; set; }
        public int Status { get; set; }
    }
    
    internal sealed class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .WithMessage("Id do pedido é obrigatório");

            RuleFor(x => x.ItemId)
                .NotNull()
                .WithMessage("Id do item é obrigatório");
            
            RuleFor(x => x.Status)
                .NotNull()
                .WithMessage("Status do item é obrigatório");
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