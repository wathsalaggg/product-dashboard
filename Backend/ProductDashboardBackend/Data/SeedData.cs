using Microsoft.EntityFrameworkCore;
using ProductDashboardBackend.Data;

namespace ProductDashboardBackend.Data
{
    public static class SqlSeedData
    {
        public static async Task InitializeWithSqlAsync(ApplicationDbContext context)
        {
            // Only seed if categories table is empty
            if (await context.Categories.AnyAsync())
            {
                Console.WriteLine("Database already seeded. Skipping...");
                return;
            }

            Console.WriteLine("Seeding database with raw SQL...");

            // Raw SQL to insert categories
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

            // Raw SQL to insert products
            var productsSql = @"
                INSERT INTO Products (Name, Description, Price, ImageUrl, CategoryId, InStock, CreatedDate) VALUES 
                -- Starters (Category 1)
                ('Garlic Bread', 'Crispy garlic-flavored bread slices with herb butter', 4.99, '/images/garlic-bread.jpg', 1, TRUE, DATE_SUB(NOW(), INTERVAL 30 DAY)),
                ('Caesar Salad', 'Fresh romaine lettuce with classic Caesar dressing, croutons, and parmesan', 6.99, '/images/caesar-salad.jpg', 1, TRUE, DATE_SUB(NOW(), INTERVAL 25 DAY)),
                ('Mozzarella Sticks', 'Crispy fried mozzarella served with marinara sauce', 7.99, '/images/mozzarella-sticks.jpg', 1, TRUE, DATE_SUB(NOW(), INTERVAL 20 DAY)),
                
                -- Main Course (Category 2)
                ('Margherita Pizza', 'Classic pizza with fresh mozzarella, tomato sauce, and basil', 11.99, '/images/margherita-pizza.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 15 DAY)),
                ('Grilled Chicken Burger', 'Juicy grilled chicken breast with lettuce, tomato, and special sauce', 9.49, '/images/chicken-burger.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 10 DAY)),
                ('Spaghetti Carbonara', 'Creamy pasta with crispy bacon, eggs, and parmesan cheese', 12.99, '/images/carbonara.jpg', 2, TRUE, DATE_SUB(NOW(), INTERVAL 5 DAY)),
                ('Grilled Salmon', 'Fresh Atlantic salmon grilled to perfection with seasonal vegetables', 15.99, '/images/grilled-salmon.jpg', 2, FALSE, DATE_SUB(NOW(), INTERVAL 2 DAY)),
                
                -- Desserts (Category 3)
                ('Chocolate Cake', 'Rich and moist chocolate cake with chocolate frosting', 5.99, '/images/chocolate-cake.jpg', 3, TRUE, DATE_SUB(NOW(), INTERVAL 18 DAY)),
                ('Tiramisu', 'Classic Italian dessert with coffee-soaked ladyfingers and mascarpone', 6.49, '/images/tiramisu.jpg', 3, TRUE, DATE_SUB(NOW(), INTERVAL 12 DAY)),
                ('Cheesecake', 'Creamy New York style cheesecake with berry compote', 6.99, '/images/cheesecake.jpg', 3, TRUE, DATE_SUB(NOW(), INTERVAL 8 DAY)),
                
                -- Beverages (Category 4)
                ('Cappuccino', 'Freshly brewed cappuccino with perfect foam', 3.99, '/images/cappuccino.jpg', 4, TRUE, DATE_SUB(NOW(), INTERVAL 22 DAY)),
                ('Fresh Orange Juice', 'Freshly squeezed orange juice, served chilled', 4.49, '/images/orange-juice.jpg', 4, TRUE, DATE_SUB(NOW(), INTERVAL 16 DAY)),
                ('Iced Tea', 'Refreshing iced tea with lemon and mint', 3.49, '/images/iced-tea.jpg', 4, FALSE, DATE_SUB(NOW(), INTERVAL 6 DAY)),
                
                -- Sides (Category 5)
                ('French Fries', 'Crispy golden fries with sea salt', 3.99, '/images/french-fries.jpg', 5, TRUE, DATE_SUB(NOW(), INTERVAL 14 DAY)),
                ('Garden Salad', 'Mixed greens with vegetables and house dressing', 4.99, '/images/garden-salad.jpg', 5, TRUE, DATE_SUB(NOW(), INTERVAL 9 DAY)),
                ('Garlic Mashed Potatoes', 'Creamy mashed potatoes with roasted garlic', 4.49, '/images/mashed-potatoes.jpg', 5, TRUE, DATE_SUB(NOW(), INTERVAL 4 DAY));
            ";

            await context.Database.ExecuteSqlRawAsync(productsSql);
            Console.WriteLine("Products seeded successfully!");
            Console.WriteLine("Seeded 5 categories and 16 products.");
        }
    }
}