namespace Order.Api.Features.Order.GetById;

internal sealed class Request
{
    public string Id { get; set; }
}

internal sealed class Response
{
    public string Id { get; set; }
    public string Client { get; set; }
    public int Status { get; set; }
    public string[] Items { get; set; }
}