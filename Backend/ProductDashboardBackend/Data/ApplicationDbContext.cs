//using Microsoft.EntityFrameworkCore;
//using ProductDashboardBackend.Models.Entities;

//namespace ProductDashboardBackend.Data
//{
//    public class ApplicationDbContext : DbContext
//    {
//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//            : base(options)
//        {
//        }

//        // DbSets (Tables)
//        public DbSet<Product> Products { get; set; }
//        public DbSet<Category> Categories { get; set; }

//        //protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//        // Relationships
//        modelBuilder.Entity<Product>()
//                .HasOne(p => p.Category)
//                .WithMany(c => c.Products)
//                .HasForeignKey(p => p.CategoryId);


//    }

//}
//}
using Microsoft.EntityFrameworkCore;
using ProductDashboard.Models;

namespace ProductDashboardBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                // Configure relationship
                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and gadgets" },
                new Category { Id = 2, Name = "Clothing", Description = "Fashion and apparel" },
                new Category { Id = 3, Name = "Home & Garden", Description = "Home improvement and garden supplies" },
                new Category { Id = 4, Name = "Sports", Description = "Sports and outdoor equipment" },
                new Category { Id = 5, Name = "Books", Description = "Books and educational materials" }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Smartphone Pro", Description = "Latest flagship smartphone with advanced features", Price = 899.99m, ImageUrl = "/images/products/phone1.jpg", CategoryId = 1, InStock = true, CreatedDate = DateTime.Now.AddDays(-30) },
                new Product { Id = 2, Name = "Wireless Headphones", Description = "Premium noise-canceling wireless headphones", Price = 299.99m, ImageUrl = "/images/products/headphones1.jpg", CategoryId = 1, InStock = true, CreatedDate = DateTime.Now.AddDays(-25) },
                new Product { Id = 3, Name = "Designer T-Shirt", Description = "Comfortable cotton t-shirt with modern design", Price = 49.99m, ImageUrl = "/images/products/tshirt1.jpg", CategoryId = 2, InStock = true, CreatedDate = DateTime.Now.AddDays(-20) },
                new Product { Id = 4, Name = "Running Shoes", Description = "Lightweight running shoes for athletes", Price = 129.99m, ImageUrl = "/images/products/shoes1.jpg", CategoryId = 2, InStock = false, CreatedDate = DateTime.Now.AddDays(-15) },
                new Product { Id = 5, Name = "Coffee Maker", Description = "Automatic drip coffee maker with programmable timer", Price = 79.99m, ImageUrl = "/images/products/coffee1.jpg", CategoryId = 3, InStock = true, CreatedDate = DateTime.Now.AddDays(-10) },
                new Product { Id = 6, Name = "Yoga Mat", Description = "Non-slip exercise mat perfect for yoga and fitness", Price = 39.99m, ImageUrl = "/images/products/yoga1.jpg", CategoryId = 4, InStock = true, CreatedDate = DateTime.Now.AddDays(-5) },
                new Product { Id = 7, Name = "Programming Book", Description = "Complete guide to modern web development", Price = 59.99m, ImageUrl = "/images/products/book1.jpg", CategoryId = 5, InStock = true, CreatedDate = DateTime.Now.AddDays(-3) },
                new Product { Id = 8, Name = "Laptop Stand", Description = "Ergonomic adjustable laptop stand for better posture", Price = 89.99m, ImageUrl = "/images/products/stand1.jpg", CategoryId = 1, InStock = true, CreatedDate = DateTime.Now.AddDays(-1) },
                new Product { Id = 9, Name = "Winter Jacket", Description = "Warm and stylish winter jacket for cold weather", Price = 199.99m, ImageUrl = "/images/products/jacket1.jpg", CategoryId = 2, InStock = true, CreatedDate = DateTime.Now },
                new Product { Id = 10, Name = "Bluetooth Speaker", Description = "Portable speaker with excellent sound quality", Price = 149.99m, ImageUrl = "/images/products/speaker1.jpg", CategoryId = 1, InStock = true, CreatedDate = DateTime.Now.AddDays(-7) },
                new Product { Id = 11, Name = "Garden Tools Set", Description = "Complete set of essential gardening tools", Price = 69.99m, ImageUrl = "/images/products/tools1.jpg", CategoryId = 3, InStock = true, CreatedDate = DateTime.Now.AddDays(-12) },
                new Product { Id = 12, Name = "Basketball", Description = "Professional grade basketball for indoor/outdoor use", Price = 24.99m, ImageUrl = "/images/products/ball1.jpg", CategoryId = 4, InStock = true, CreatedDate = DateTime.Now.AddDays(-18) }
            );
        }
    }
}