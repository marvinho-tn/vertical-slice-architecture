namespace Worker.Event.Order.ItemStatusUpdated;

internal sealed class Message
{
    public string OrderId { get; set; }
    public string ItemId { get; set; }
    public int Status { get; set; }
}