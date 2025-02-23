﻿using Common.Data;
using FastEndpoints;
using FluentValidation;

namespace Inventory.Api.Features.Product.ControlStockHistory
{
    internal sealed class Request
    {
        public string Id { get; set; }
        public int OperationType { get; set; }
        public int Quantity { get; set; }
    }

    internal sealed class Validator : Validator<Request>
    {
        public Validator(IDbContext dbContext)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id do produto é obrigatório");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantidade deve ser maior que zero");

            RuleFor(x => (ProductEntity.ProductStockEntity.StockOperationType) x.OperationType)
                .IsInEnum()
                .WithMessage("Tipo de operação inválido")
                .Must((request, cancellation) =>
                {
                    if (request.OperationType == (int) ProductEntity.ProductStockEntity.StockOperationType.Decrease)
                    {
                        var product = dbContext.GetById<ProductEntity>(request.Id);

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
