using FastEndpoints;
using FluentValidation;

namespace Inventory.Api.Features.Product.SearchProduct
{
    internal sealed class Request
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Fields { get; set; }

        public Dictionary<string, string> FieldsAsDictionary => 
            string.IsNullOrWhiteSpace(Fields) ? [] : Fields.Split(',').Select(f => f.Split(':')).ToDictionary(f => f[0], f => f[1]);
    }

    internal sealed class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Pagina deve ser maior ou igual a 1");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Tamanho da pagina deve ser maior ou igual a 1");

            RuleForEach(x => x.FieldsAsDictionary)
                .Must(f => f.Key.Length > 0 && f.Value.Length > 0)
                .WithMessage("Campos devem ser informados no formato 'campo:valor'");

            RuleForEach(x => x.FieldsAsDictionary)
                .Must(f => typeof(ProductEntity).GetProperties().Select(p => p.Name).Contains(f.Key))
                .WithMessage("Nome do campo deve estar contido no domínio de produto");
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