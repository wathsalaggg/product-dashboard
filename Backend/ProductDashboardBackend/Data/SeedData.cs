using Microsoft.EntityFrameworkCore;
using ProductDashboardBackend.Data;

namespace ProductDashboardBackend.Data
{
    public static class SqlSeedData
    {
        public static async Task InitializeWithSqlAsync(ApplicationDbContext context)
        {
            Console.WriteLine("Checking database seed status...");

            // Seed categories if they don't exist
            if (!await context.Categories.AnyAsync())
            {
                Console.WriteLine("Seeding categories...");
                await SeedCategoriesAsync(context);
            }
            else
            {
                Console.WriteLine("Categories already exist, skipping category seeding.");
            }

            // Check product count and seed if needed
            var productCount = await context.Products.CountAsync();
            Console.WriteLine($"Current product count: {productCount}");

            if (productCount < 32) // Expected total products
            {
                Console.WriteLine($"Seeding additional products... (current: {productCount}, target: 32)");
                await SeedProductsAsync(context);
            }
            else
            {
                Console.WriteLine("All products already seeded.");
            }
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            var categoriesSql = @"
                INSERT INTO Categories (Name, Description) VALUES 
                ('Starters', 'Appetizers and light bites to begin your meal'),
                ('Main Course', 'Delicious and satisfying main dishes'),
                ('Desserts', 'Sweet treats and delightful endings to your meal'),
                ('Beverages', 'Refreshing hot and cold drinks'),
                ('Sides', 'Perfect accompaniments to your main dishes');
            ";

            await context.Database.ExecuteSqlRawAsync(categoriesSql);
            Console.WriteLine("Categories seeded successfully!");
        }

        private static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            // First batch of products
            if (!await context.Products.AnyAsync(p => p.Name == "Garlic Bread"))
            {
                var initialProductsSql = @"
                    -- Starters (Category 1)
                    INSERT INTO Products (Name, Description, Price, ImageUrl, CategoryId, InStock, CreatedDate) VALUES
                    ('Garlic Bread', 'Crispy garlic-flavored bread slices with herb butter', 850.00, '/images/products/garlic-bread.jpg', 1, TRUE, DATE_SUB(NOW(), INTERVAL 30 DAY)),
                    ('Caesar Salad', 'Fresh romaine lettuce with classic Caesar dressing, croutons, and parmesan', 1650.00, '/images/products/caesar-salad.jpg', 1, TRUE, DATE_SUB(NOW(), INTERVAL 25 DAY)),
                    ('Mozzarella Sticks', 'Crispy fried mozzarella served with marinara sauce', 1200.00, '/images/products/mozzarella-sticks.jpg', 1, TRUE, DATE_SUB(NOW(), INTERVAL 20 DAY)),

                    -- Main Course (Category 2)
                    ('Margherita Pizza', 'Classic pizza with fresh mozzarella, tomato sauce, and basil', 3250.00, '/images/products/margherita-pizza.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 15 DAY)),
                    ('Grilled Chicken Burger', 'Juicy grilled chicken breast with lettuce, tomato, and special sauce', 2800.00, '/images/products/chicken-burger.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 10 DAY)),
                    ('Spaghetti Carbonara', 'Creamy pasta with crispy bacon, eggs, and parmesan cheese', 3500.00, '/images/products/carbonara.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 5 DAY)),
                    ('Grilled Salmon', 'Fresh Atlantic salmon grilled to perfection with seasonal vegetables', 4800.00, '/images/products/grilled-salmon.jpg', 2, FALSE, DATE_SUB(NOW(), INTERVAL 2 DAY)),

                    -- Desserts (Category 3)
                    ('Chocolate Cake', 'Rich and moist chocolate cake with chocolate frosting', 1300.00, '/images/products/chocolate-cake.jpg', 3, TRUE, DATE_SUB(NOW(), INTERVAL 18 DAY)),
                    ('Tiramisu', 'Classic Italian dessert with coffee-soaked ladyfingers and mascarpone', 1500.00, '/images/products/tiramisu.jpg', 3, TRUE, DATE_SUB(NOW(), INTERVAL 12 DAY)),
                    ('Cheesecake', 'Creamy New York style cheesecake with berry compote', 1600.00, '/images/products/cheesecake.jpg', 3, TRUE, DATE_SUB(NOW(), INTERVAL 8 DAY)),

                    -- Beverages (Category 4)
                    ('Cappuccino', 'Freshly brewed cappuccino with perfect foam', 950.00, '/images/products/cappuccino.jpg', 4, TRUE, DATE_SUB(NOW(), INTERVAL 22 DAY)),
                    ('Fresh Orange Juice', 'Freshly squeezed orange juice, served chilled', 750.00, '/images/products/orange-juice.jpg', 4, TRUE, DATE_SUB(NOW(), INTERVAL 16 DAY)),
                    ('Iced Tea', 'Refreshing iced tea with lemon and mint', 700.00, '/images/products/iced-tea.jpg', 4, FALSE, DATE_SUB(NOW(), INTERVAL 6 DAY)),

                    -- Sides (Category 5)
                    ('French Fries', 'Crispy golden fries with sea salt', 850.00, '/images/products/french-fries.jpg', 5, TRUE, DATE_SUB(NOW(), INTERVAL 14 DAY)),
                    ('Garden Salad', 'Mixed greens with vegetables and house dressing', 1100.00, '/images/products/garden-salad.jpg', 5, TRUE, DATE_SUB(NOW(), INTERVAL 9 DAY)),
                    ('Garlic Mashed Potatoes', 'Creamy mashed potatoes with roasted garlic', 950.00, '/images/products/mashed-potatoes.jpg', 5, TRUE, DATE_SUB(NOW(), INTERVAL 4 DAY));
                ";

                await context.Database.ExecuteSqlRawAsync(initialProductsSql);
                Console.WriteLine("Initial products seeded successfully!");
            }

            // Second batch of products
            if (!await context.Products.AnyAsync(p => p.Name == "Bruschetta"))
            {
                var moreProductsSql = @"
                    -- More Starters (Category 1)
                    INSERT INTO Products (Name, Description, Price, ImageUrl, CategoryId, InStock, CreatedDate) VALUES
                    ('Bruschetta', 'Toasted bread topped with tomatoes, basil, and olive oil', 1250.00, '/images/products/bruschetta.jpg', 1, TRUE, DATE_SUB(NOW(), INTERVAL 28 DAY)),
                    ('Stuffed Mushrooms', 'Mushrooms filled with cheese and herbs, baked golden', 1350.00, '/images/products/stuffed-mushrooms.jpg', 1, TRUE, DATE_SUB(NOW(), INTERVAL 24 DAY)),
                    ('Chicken Satay', 'Grilled skewers of marinated chicken with peanut sauce', 1550.00, '/images/products/chicken-satay.jpg', 1, TRUE, DATE_SUB(NOW(), INTERVAL 20 DAY)),
                    ('Coleslaw', 'Creamy shredded cabbage and carrot slaw', 850.00, '/images/products/coleslaw.jpg', 1, TRUE, DATE_SUB(NOW(), INTERVAL 10 DAY)),

                    -- More Main Course (Category 2)
                    ('Beef Burrito', 'Flour tortilla stuffed with seasoned beef, rice, and beans', 2800.00, '/images/products/beef-burrito.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 12 DAY)),
                    ('Beef Stroganoff', 'Classic Russian dish with beef strips in creamy mushroom sauce', 3900.00, '/images/products/beef-stroganoff.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 8 DAY)),
                    ('Chicken Parmesan', 'Breaded chicken with marinara and melted cheese', 3600.00, '/images/products/chicken-parmesan.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 6 DAY)),
                    ('Pulled Pork', 'Slow-cooked pulled pork with smoky BBQ sauce', 4200.00, '/images/products/pulled-pork.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 3 DAY)),
                    ('Roasted Potatoes', 'Golden roasted potatoes with herbs and garlic', 1500.00, '/images/products/roasted-potatoes.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 2 DAY)),
                    ('Vegetable Curry', 'Spiced seasonal vegetables simmered in coconut curry', 2500.00, '/images/products/vegetable-curry.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 5 DAY)),
                    ('Vegetable Paella', 'Spanish rice dish with saffron and mixed vegetables', 3100.00, '/images/products/vegetable-paella.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 7 DAY)),

                    -- More Desserts (Category 3)
                    ('Apple Pie', 'Classic warm apple pie with a flaky crust', 1400.00, '/images/products/apple-pie.jpg', 3, TRUE, DATE_SUB(NOW(), INTERVAL 15 DAY)),
                    ('Creme Brulee', 'Silky custard topped with caramelized sugar crust', 1600.00, '/images/products/creme-brulee.jpg', 3, TRUE, DATE_SUB(NOW(), INTERVAL 13 DAY)),
                    ('Chocolate Mousse', 'Light and airy chocolate dessert with whipped cream', 1350.00, '/images/products/chocolate-mousse.jpg', 3, TRUE, DATE_SUB(NOW(), INTERVAL 9 DAY)),
                    ('Fruit Tart', 'Buttery tart filled with custard and fresh fruits', 1500.00, '/images/products/fruit-tart.jpg', 3, TRUE, DATE_SUB(NOW(), INTERVAL 6 DAY)),

                    -- More Beverages (Category 4)
                    ('Green Tea', 'Soothing cup of freshly brewed green tea', 600.00, '/images/products/green-tea.jpg', 4, TRUE, DATE_SUB(NOW(), INTERVAL 21 DAY)),
                    ('Iced Coffee', 'Cold coffee served over ice with milk or cream', 850.00, '/images/products/iced-coffee.jpg', 4, TRUE, DATE_SUB(NOW(), INTERVAL 18 DAY)),
                    ('Lemonade', 'Refreshing homemade lemonade with a zesty twist', 650.00, '/images/products/lemonade.jpg', 4, TRUE, DATE_SUB(NOW(), INTERVAL 12 DAY)),
                    ('Mango Smoothie', 'Creamy smoothie with fresh ripe mangoes', 1200.00, '/images/products/mango-smoothie.jpg', 4, TRUE, DATE_SUB(NOW(), INTERVAL 10 DAY));
                ";

                await context.Database.ExecuteSqlRawAsync(moreProductsSql);
                Console.WriteLine("Additional products seeded successfully!");
            }

            var finalCount = await context.Products.CountAsync();
            Console.WriteLine($"Final product count: {finalCount}");
        }

        // Method to force re-seed all data (use with caution)
        public static async Task ForceReseedAsync(ApplicationDbContext context)
        {
            Console.WriteLine("Force reseeding - clearing existing data...");

            await context.Database.ExecuteSqlRawAsync("DELETE FROM Products");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Categories");

            Console.WriteLine("Existing data cleared. Reseeding...");
            await InitializeWithSqlAsync(context);
        }
    }
}