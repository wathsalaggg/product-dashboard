namespace ProductDashboard.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Product?.Price * Quantity ?? 0;
    }
}