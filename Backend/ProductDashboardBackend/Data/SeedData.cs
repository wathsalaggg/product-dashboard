using ProductDashboardBackend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProductDashboardBackend.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            var categories = new List<Category>
            {
                new Category { Name = "Starters", Description = "Appetizers and light bites", IconName = "utensils" },
                new Category { Name = "Main Course", Description = "Delicious main dishes", IconName = "drumstick-bite" },
                new Category { Name = "Desserts", Description = "Sweet treats and cakes", IconName = "ice-cream" },
                new Category { Name = "Beverages", Description = "Hot and cold drinks", IconName = "coffee" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            var products = new List<Product>
            {
                // Starters
                new Product { Name = "Garlic Bread", Description = "Crispy garlic-flavored bread slices", Price = 4.99m, ImageUrl = "/images/garlic-bread.jpg", CategoryId = 1, StockQuantity = 50, Brand = "Chef’s Special", Rating = 4.3 },
                new Product { Name = "Caesar Salad", Description = "Fresh romaine lettuce with Caesar dressing", Price = 6.99m, ImageUrl = "/images/caesar-salad.jpg", CategoryId = 1, StockQuantity = 40, Brand = "Fresh Greens", Rating = 4.5 },

                // Main Course
                new Product { Name = "Margherita Pizza", Description = "Classic cheese and tomato pizza", Price = 11.99m, ImageUrl = "/images/margherita-pizza.jpg", CategoryId = 2, StockQuantity = 20, Brand = "Pizza Hub", Rating = 4.7 },
                new Product { Name = "Grilled Chicken Burger", Description = "Juicy grilled chicken with fries", Price = 9.49m, ImageUrl = "/images/chicken-burger.jpg", CategoryId = 2, StockQuantity = 25, Brand = "Burger Shack", Rating = 4.6 },

                // Desserts
                new Product { Name = "Chocolate Cake", Description = "Rich and moist chocolate cake slice", Price = 5.99m, ImageUrl = "/images/chocolate-cake.jpg", CategoryId = 3, StockQuantity = 30, Brand = "Sweet Bites", Rating = 4.8 },

                // Beverages
                new Product { Name = "Cappuccino", Description = "Freshly brewed cappuccino coffee", Price = 3.99m, ImageUrl = "/images/cappuccino.jpg", CategoryId = 4, StockQuantity = 40, Brand = "Coffee House", Rating = 4.7 }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
