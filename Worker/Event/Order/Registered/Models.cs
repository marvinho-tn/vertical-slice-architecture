namespace Worker.Event.Order.Registered;

internal sealed class Message
{
    public string OrderID { get; set; }
    public string[] Items { get; set; }
}