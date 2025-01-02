namespace Worker.Events.Order.Registered;

internal sealed class Message
{
    public string OrderID { get; set; }
    public string[] Items { get; set; }
}