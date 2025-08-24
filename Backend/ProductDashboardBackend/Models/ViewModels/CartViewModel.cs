namespace ProductDashboard.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(x => x.TotalPrice);
        public int TotalItems => Items.Sum(x => x.Quantity);
    }
}