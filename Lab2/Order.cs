namespace CafeOrderApp
{
public class Order
{
    public int OrderId { get; set; }
    public string ClientName { get; set; }
    public string OrderType { get; set; }
    public List<OrderItem> Items { get; set; }

    public string FullOrderDetails
{
    get
    {
        var details = $"{ClientName} ({OrderType}):\n";
        foreach (var item in Items)
        {
            details += $"• {item.Dish.Name} x{item.Quantity}";
            if (item.Options.Count > 0)
            {
                details += $" [{string.Join(", ", item.Options)}]";
            }
            details += "\n";
        }
        return details;
    }
}

}
}