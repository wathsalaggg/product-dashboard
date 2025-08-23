﻿namespace ProductDashboardBackend.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; } 
        public string ImageUrl { get; set; } = null!;
        public int StockQuantity { get; set; }
        public string Brand { get; set; } = null!;
        public double Rating { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
