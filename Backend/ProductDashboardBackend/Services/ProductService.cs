// Data/ProductService.cs - Entity Framework Version
using Microsoft.EntityFrameworkCore;
using ProductDashboard.Models;
using ProductDashboardBackend.Data;

namespace ProductDashboard.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Product> GetProducts(string searchTerm = null, int? categoryId = null, int page = 1, int pageSize = 9)
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

            return query.OrderByDescending(p => p.CreatedDate)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();
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

        public async Task<List<Product>> GetProductsAsync(string searchTerm = null, int? categoryId = null, int page = 1, int pageSize = 9)
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

            return await query.OrderByDescending(p => p.CreatedDate)
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
    }
}