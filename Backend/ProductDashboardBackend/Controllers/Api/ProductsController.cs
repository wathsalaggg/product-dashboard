// Controllers/Api/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using ProductDashboard.Models;
using ProductDashboard.Services;
using ProductDashboardBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace ProductDashboard.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;
        private const int PageSize = 8;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger, ProductService productService)
        {
            _productService = productService;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ProductListApiResponse>> GetProducts(
            [FromQuery] string searchTerm = "",
            [FromQuery] int? categoryId = null,
            [FromQuery] int page = 1)
        {
            try
            {
                var products = _productService.GetProducts(searchTerm, categoryId, page, PageSize);
                var totalProducts = _productService.GetProductCount(searchTerm, categoryId);

                var response = new ProductListApiResponse
                {
                    Products = products.Select(p => new ProductDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category?.Name,
                        CreatedDate = p.CreatedDate
                    }).ToList(),
                    CurrentPage = page,
                    TotalProducts = totalProducts,
                    TotalPages = (int)Math.Ceiling((double)totalProducts / PageSize),
                    PageSize = PageSize
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, new { error = "Failed to retrieve products" });
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            try
            {
                var categories = _productService.GetCategories();
                var categoryDtos = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();

                return Ok(categoryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return StatusCode(500, new { error = "Failed to retrieve categories" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var product = _productService.GetProduct(id);
                if (product == null)
                {
                    return NotFound();
                }

                var productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name,
                    CreatedDate = product.CreatedDate
                };

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {ProductId}", id);
                return StatusCode(500, new { error = "Failed to retrieve product" });
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductService _productService;
        private readonly ILogger<CartController> _logger;

        public CartController(ApplicationDbContext context, ILogger<CartController> logger, ProductService productService)
        {
            _context = context;
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<CartApiResponse>> GetCart()
        {
            try
            {
                var cartItems = GetCartItems();
                var response = new CartApiResponse
                {
                    Items = cartItems.Select(item => new CartItemDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        ProductName = item.Product?.Name,
                        ProductImageUrl = item.Product?.ImageUrl,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                };

                response.Subtotal = response.Items.Sum(item => item.TotalPrice);
                response.Tax = response.Subtotal * 0.1m;
                response.Total = response.Subtotal + response.Tax;
                response.TotalItems = response.Items.Sum(item => item.Quantity);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart");
                return StatusCode(500, new { error = "Failed to retrieve cart" });
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<CartActionResponse>> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                var product = _productService.GetProduct(request.ProductId);
                if (product == null)
                {
                    return NotFound(new { error = "Product not found" });
                }

                var sessionId = GetOrCreateSessionId();
                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.ProductId == request.ProductId && ci.SessionId == sessionId);

                if (existingItem != null)
                {
                    existingItem.Quantity += request.Quantity;
                }
                else
                {
                    var newItem = new CartItem
                    {
                        ProductId = request.ProductId,
                        Quantity = request.Quantity,
                        UnitPrice = product.Price,
                        SessionId = sessionId,
                        AddedDate = DateTime.UtcNow
                    };
                    _context.CartItems.Add(newItem);
                }

                await _context.SaveChangesAsync();

                var cartItems = GetCartItems();
                var totalItems = cartItems.Sum(item => item.Quantity);

                return Ok(new CartActionResponse
                {
                    Success = true,
                    TotalItems = totalItems,
                    Message = $"{product.Name} added to cart"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");
                return StatusCode(500, new CartActionResponse
                {
                    Success = false,
                    Message = "Error adding to cart"
                });
            }
        }

        [HttpPut("{itemId}")]
        public async Task<ActionResult<CartActionResponse>> UpdateCartItem(int itemId, [FromBody] UpdateCartItemRequest request)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(itemId);
                if (cartItem == null)
                {
                    return NotFound(new { error = "Item not found in cart" });
                }

                if (request.Quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = request.Quantity;
                }

                await _context.SaveChangesAsync();

                var cartItems = GetCartItems();
                var totalItems = cartItems.Sum(item => item.Quantity);
                var subtotal = cartItems.Sum(item => item.TotalPrice);
                var tax = subtotal * 0.1m;
                var total = subtotal + tax;

                return Ok(new CartActionResponse
                {
                    Success = true,
                    TotalItems = totalItems,
                    Subtotal = subtotal,
                    Tax = tax,
                    Total = total,
                    Message = "Cart updated"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item");
                return StatusCode(500, new CartActionResponse
                {
                    Success = false,
                    Message = "Error updating cart"
                });
            }
        }

        [HttpDelete("{itemId}")]
        public async Task<ActionResult<CartActionResponse>> RemoveFromCart(int itemId)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(itemId);
                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);
                    await _context.SaveChangesAsync();
                }

                var cartItems = GetCartItems();
                var totalItems = cartItems.Sum(item => item.Quantity);
                var subtotal = cartItems.Sum(item => item.TotalPrice);
                var tax = subtotal * 0.1m;
                var total = subtotal + tax;

                return Ok(new CartActionResponse
                {
                    Success = true,
                    TotalItems = totalItems,
                    Subtotal = subtotal,
                    Tax = tax,
                    Total = total,
                    Message = "Item removed from cart"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from cart");
                return StatusCode(500, new CartActionResponse
                {
                    Success = false,
                    Message = "Error removing from cart"
                });
            }
        }

        [HttpDelete("clear")]
        public async Task<ActionResult<CartActionResponse>> ClearCart()
        {
            try
            {
                var sessionId = GetOrCreateSessionId();
                var cartItems = await _context.CartItems
                    .Where(ci => ci.SessionId == sessionId)
                    .ToListAsync();

                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                return Ok(new CartActionResponse
                {
                    Success = true,
                    TotalItems = 0,
                    Subtotal = 0,
                    Tax = 0,
                    Total = 0,
                    Message = "Cart cleared"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                return StatusCode(500, new CartActionResponse
                {
                    Success = false,
                    Message = "Error clearing cart"
                });
            }
        }


        private List<CartItem> GetCartItems()
        {
            var sessionId = GetOrCreateSessionId();
            return _context.CartItems
                .Where(ci => ci.SessionId == sessionId)
                .Include(ci => ci.Product)
                .ThenInclude(p => p.Category)
                .ToList();
        }

        private string GetOrCreateSessionId()
        {
            var sessionId = HttpContext.Session.GetString("CartSessionId");
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartSessionId", sessionId);
            }
            return sessionId;
        }
    }

    // DTOs for API responses
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class ProductListApiResponse
    {
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int CurrentPage { get; set; }
        public int TotalProducts { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }

    public class CartApiResponse
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public int TotalItems { get; set; }
    }

    public class CartActionResponse
    {
        public bool Success { get; set; }
        public int TotalItems { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemRequest
    {
        public int Quantity { get; set; }
    }


}