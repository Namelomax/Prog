public class Order
{
    public int OrderId { get; set; }
    public string ClientName { get; set; }
    public string Dish { get; set; }
    public string OrderType { get; set; }
    public List<string> Options { get; set; }
}
