// Data/ProductService.cs - Entity Framework Version
using Microsoft.EntityFrameworkCore;
using ProductDashboard.Models;
using ProductDashboardBackend.Data;
using System.Diagnostics;

namespace ProductDashboard.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Product> GetProducts(string searchTerm = null, int? categoryId = null, int page = 1, int pageSize = 8)
        {
            Debug.WriteLine($"=== PRODUCT SERVICE DEBUG ===");
            Debug.WriteLine($"Page: {page}, PageSize: {pageSize}");
            Debug.WriteLine($"Skip: {(page - 1) * pageSize}, Take: {pageSize}");

            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) ||
                                        p.Description.Contains(searchTerm));
                Debug.WriteLine($"Search filter: '{searchTerm}'");
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
                Debug.WriteLine($"Category filter: {categoryId.Value}");
            }

            // Get total count for debugging
            var totalCount = query.Count();
            Debug.WriteLine($"Total products matching filters: {totalCount}");

            // Get ALL products in the current order for debugging
            var allProductsInOrder = query.OrderByDescending(p => p.CreatedDate)
                                         .ThenBy(p => p.Id)
                                         .ToList();

            Debug.WriteLine($"All products in order (Total: {allProductsInOrder.Count}):");
            for (int i = 0; i < allProductsInOrder.Count; i++)
            {
                Debug.WriteLine($"{i + 1}. ID: {allProductsInOrder[i].Id}, Name: {allProductsInOrder[i].Name}, Created: {allProductsInOrder[i].CreatedDate}");
            }

            // Apply pagination
            var result = query.OrderByDescending(p => p.CreatedDate)
                           .ThenBy(p => p.Id)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

            Debug.WriteLine($"Returning {result.Count} products for page {page}:");
            foreach (var product in result)
            {
                Debug.WriteLine($"- ID: {product.Id}, Name: {product.Name}, Created: {product.CreatedDate}");
            }

            // Verify pagination logic
            if (page > 1 && result.Count > 0)
            {
                var expectedFirstProduct = allProductsInOrder.Skip((page - 1) * pageSize).FirstOrDefault();
                if (expectedFirstProduct != null && result[0].Id != expectedFirstProduct.Id)
                {
                    Debug.WriteLine($"!!! PAGINATION ERROR !!!");
                    Debug.WriteLine($"Expected first product ID: {expectedFirstProduct.Id}");
                    Debug.WriteLine($"Actual first product ID: {result[0].Id}");
                }
            }

            Debug.WriteLine($"=== END DEBUG ===\n");

            return result;
        }

        public int GetProductCount(string searchTerm = null, int? categoryId = null)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) ||
                                        p.Description.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return query.Count();
        }

        public Product GetProduct(int id)
        {
            return _context.Products.Include(p => p.Category)
                                  .FirstOrDefault(p => p.Id == id);
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.OrderBy(c => c.Name).ToList();
        }

        public async Task<Product> GetProductAsync(int id)
        {
            return await _context.Products.Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetProductsAsync(string searchTerm = null, int? categoryId = null, int page = 1, int pageSize = 8)
        {
            Debug.WriteLine($"=== PRODUCT SERVICE ASYNC DEBUG ===");
            Debug.WriteLine($"Page: {page}, PageSize: {pageSize}");

            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) ||
                                        p.Description.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // FIXED: Added secondary ordering by ID for consistent pagination
            return await query.OrderByDescending(p => p.CreatedDate)
                             .ThenBy(p => p.Id)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync();
        }

        public async Task<int> GetProductCountAsync(string searchTerm = null, int? categoryId = null)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) ||
                                        p.Description.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        }

        // DEBUG METHOD: Get raw SQL query being generated
        public string GetProductsQueryDebug(string searchTerm = null, int? categoryId = null, int page = 1, int pageSize = 8)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) ||
                                        p.Description.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var orderedQuery = query.OrderByDescending(p => p.CreatedDate)
                                  .ThenBy(p => p.Id)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize);

            return orderedQuery.ToQueryString();
        }
    }
}