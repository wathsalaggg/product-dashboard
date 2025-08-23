using Microsoft.EntityFrameworkCore;
using ProductDashboardBackend.Data;
using System;

public interface IProductService
{
    Task<PagedResult<Product>> GetFilteredProductsAsync(ProductFilterParameters filters);
}

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Product>> GetFilteredProductsAsync(ProductFilterParameters filters)
    {
        var query = _context.Products.AsQueryable();

        // Apply filters
        query = ApplyFilters(query, filters);

        // Get total count for pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var products = await query
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = products,
            TotalCount = totalCount,
            Page = filters.Page,
            PageSize = filters.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / filters.PageSize)
        };
    }

    private IQueryable<Product> ApplyFilters(IQueryable<Product> query, ProductFilterParameters filters)
    {
        // Search term filter
        if (!string.IsNullOrWhiteSpace(filters.S))
        {
            query = query.Where(p => p.Name.Contains(filters.S) ||
                                   p.Description.Contains(filters.S));
        }

        // Category filter
        if (filters.CategoryIds != null && filters.CategoryIds.Any())
        {
            query = query.Where(p => p.CategoryIds.Any(categoryId =>
                filters.CategoryIds.Contains(categoryId)));
        }

        // Rating filters
        if (filters.RatingGte.HasValue)
        {
            query = query.Where(p => p.Rating >= filters.RatingGte.Value);
        }

        if (filters.RatingLte.HasValue)
        {
            query = query.Where(p => p.Rating <= filters.RatingLte.Value);
        }

        // Quantity filter
        if (filters.QuantityGte.HasValue)
        {
            query = query.Where(p => p.Quantity >= filters.QuantityGte.Value);
        }

        return query;
    }
}