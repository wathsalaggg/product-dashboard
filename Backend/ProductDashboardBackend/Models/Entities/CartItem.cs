using System;
using System.ComponentModel.DataAnnotations;

namespace ProductDashboardBackend.Models.Entities
{
    public class CartItem
    {
        [Key] 
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        //public decimal TotalPrice => Product.Price * Quantity;

        public DateTime AddedAt { get; set; }
    }
}
