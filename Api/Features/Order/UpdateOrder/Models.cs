﻿using FastEndpoints;
using FluentValidation;

namespace Api.Features.Order.UpdateOrder
{
    internal sealed class Request
    {
        public string Id { get; set; }
        public string Client { get; set; }
        public string[] Items { get; set; }
    }

    internal sealed class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .WithMessage("Id do pedido é obrigatório");

            RuleFor(x => x.Client)
                .NotNull()
                .WithMessage("Cliente do pedido é obrigatório");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Pedido deve conter ao menos um item");

            RuleForEach(x => x.Items)
                .NotEmpty()
                .WithMessage("Nome do item do pedido é obrigatório");
        }
    }

    internal sealed class Response
    {
        public string Id { get; set; }
        public string Client { get; set; }
        public string[] Items { get; set; }
    }
}