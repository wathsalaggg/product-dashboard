public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int Quantity { get; set; }
    public List<int> CategoryIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}