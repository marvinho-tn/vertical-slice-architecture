namespace Worker.Event.Product.OutOfStock;

internal sealed class Request
{
    public string Id { get; set; }
    public string Client { get; set; }
    public int Status { get; set; }
    public string[] Items { get; set; }
}

internal sealed class Response
{
    public string Id { get; set; }
    public string Client { get; set; }
    public int Status { get; set; }
    public string[] Items { get; set; }
}

internal sealed class Message
{
    public string SourceOrderID { get; set; }
    public string ProductID { get; set; }
}