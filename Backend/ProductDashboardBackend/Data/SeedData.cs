using Microsoft.EntityFrameworkCore;
using ProductDashboardBackend.Models.Entities;

namespace ProductDashboardBackend.Data
{
    public static class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Main Course", Description = "Delicious main dishes", IconName = "" },
                new Category { Id = 2, Name = "Desserts", Description = "Sweet treats", IconName = "" }
            );

            // Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Pizza", Description = "Cheesy pizza", Price = 12.5m, ImageUrl = "pizza.jpg", StockQuantity = 10, Brand = "Domino's", Rating = 4.5, CategoryId = 1 },
                new Product { Id = 2, Name = "Chocolate Cake", Description = "Rich chocolate cake", Price = 6.0m, ImageUrl = "cake.jpg", StockQuantity = 5, Brand = "Bakery", Rating = 4.8, CategoryId = 2 }
            );
        }
    }
}
