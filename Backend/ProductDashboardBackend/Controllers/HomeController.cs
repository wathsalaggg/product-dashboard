using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductDashboard.Models;
using ProductDashboard.Models.ViewModels;
using ProductDashboard.Services;
using ProductDashboardBackend.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;

namespace ProductDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly ICompositeViewEngine _viewEngine;
        private const int PageSize = 8; // Consistent page size

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, ICompositeViewEngine viewEngine)
        {
            _productService = new ProductService(context);
            _context = context;
            _logger = logger;
            _viewEngine = viewEngine;
        }

        public IActionResult Index()
        {
            // Check if React frontend is requested
            bool useReact = Request.Query.ContainsKey("react") ||
                           Request.Cookies.ContainsKey("prefersReact") ||
                           // FIX: Use injected IConfiguration instead of 'builder'
                           (HttpContext.RequestServices.GetService(typeof(IConfiguration)) is IConfiguration config &&
                            config.GetValue<bool>("DefaultToReact", false));

            if (useReact)
            {
                // Set a cookie to remember the preference
                Response.Cookies.Append("prefersReact", "true", new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true
                });

                // If using SPA services, this will be handled automatically
                // Otherwise, serve a simple HTML page that loads the React app
                return View("ReactApp");
            }

            // Return the main view with categories for the filter dropdown
            var viewModel = new ProductListViewModel
            {
                Categories = _productService.GetCategories()
            };
            return View(viewModel);
        }

        public PartialViewResult HomeContent()
        {
            var viewModel = new ProductListViewModel
            {
                Categories = _productService.GetCategories()
            };
            return PartialView("_HomeContent", viewModel);
        }

        public PartialViewResult ProductList(string searchTerm = "", int? categoryId = null, int page = 1)
        {
            try
            {
                var products = _productService.GetProducts(searchTerm, categoryId, page, PageSize);
                var totalProducts = _productService.GetProductCount(searchTerm, categoryId);

                var viewModel = new ProductListViewModel
                {
                    Products = products,
                    CurrentPage = page,
                    TotalProducts = totalProducts,
                    TotalPages = (int)Math.Ceiling((double)totalProducts / PageSize),
                    SearchTerm = searchTerm,
                    CategoryId = categoryId,
                    Categories = _productService.GetCategories(),
                    PageSize = PageSize
                };

                return PartialView("~/Views/Product/_ProductList.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProductList action");
                Console.WriteLine($"Error in ProductList: {ex.Message}");

                return PartialView("~/Views/Product/_ProductList.cshtml", new ProductListViewModel
                {
                    Products = new List<Product>(),
                    CurrentPage = 1,
                    TotalProducts = 0,
                    TotalPages = 0,
                    Categories = _productService.GetCategories(),
                    PageSize = PageSize
                });
            }
        }

        public PartialViewResult Promotions()
        {
            return PartialView("_Promotions");
        }

        public PartialViewResult Community()
        {
            return PartialView("_Community");
        }

        [HttpGet]
        public IActionResult Cart()
        {
            try
            {
                var cartItems = GetCartItems();
                var viewModel = new CartViewModel
                {
                    CartItems = cartItems,
                    Subtotal = cartItems.Sum(item => item.TotalPrice),
                    TotalItems = cartItems.Sum(item => item.Quantity)
                };

                viewModel.Tax = viewModel.Subtotal * 0.1m;
                viewModel.Total = viewModel.Subtotal + viewModel.Tax;

                // Specify the full path to the view in the Product folder
                return PartialView("~/Views/Product/_Cart.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Cart action");

                // Return empty cart view on error
                var emptyViewModel = new CartViewModel
                {
                    CartItems = new List<CartItem>(),
                    Subtotal = 0,
                    TotalItems = 0,
                    Tax = 0,
                    Total = 0
                };

                return PartialView("~/Views/Product/_Cart.cshtml", emptyViewModel);
            }
        }

        [HttpPost]
        public JsonResult AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var product = _productService.GetProduct(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                var sessionId = GetOrCreateSessionId();
                var existingItem = _context.CartItems
                    .FirstOrDefault(ci => ci.ProductId == productId && ci.SessionId == sessionId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    var newItem = new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = product.Price,
                        SessionId = sessionId,
                        AddedDate = DateTime.UtcNow
                    };
                    _context.CartItems.Add(newItem);
                }

                _context.SaveChanges();

                var cartItems = GetCartItems();
                var totalItems = cartItems.Sum(item => item.Quantity);

                return Json(new
                {
                    success = true,
                    totalItems = totalItems,
                    message = $"{product.Name} added to cart"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddToCart action");
                return Json(new { success = false, message = "Error adding to cart: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateCartItem(int itemId, int quantity)
        {
            try
            {
                var cartItem = _context.CartItems.Find(itemId);
                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Item not found in cart" });
                }

                if (quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                }

                _context.SaveChanges();

                var cartItems = GetCartItems();
                var totalItems = cartItems.Sum(item => item.Quantity);
                var itemTotal = quantity > 0 ? cartItem.UnitPrice * quantity : 0;
                var subtotal = cartItems.Sum(item => item.TotalPrice);
                var tax = subtotal * 0.1m;
                var total = subtotal + tax;

                return Json(new
                {
                    success = true,
                    totalItems = totalItems,
                    itemTotal = itemTotal.ToString("N2"), // This will be displayed with $ in the view
                    subtotal = subtotal.ToString("N2"),
                    tax = tax.ToString("N2"),
                    total = total.ToString("N2")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCartItem action");
                return Json(new { success = false, message = "Error updating cart: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RemoveFromCart(int itemId)
        {
            try
            {
                var cartItem = _context.CartItems.Find(itemId);
                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);
                    _context.SaveChanges();
                }

                var cartItems = GetCartItems();
                var totalItems = cartItems.Sum(item => item.Quantity);
                var subtotal = cartItems.Sum(item => item.TotalPrice);
                var tax = subtotal * 0.1m;
                var total = subtotal + tax;

                return Json(new
                {
                    success = true,
                    totalItems = totalItems,
                    subtotal = subtotal.ToString("N2"),
                    tax = tax.ToString("N2"),
                    total = total.ToString("N2")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RemoveFromCart action");
                return Json(new { success = false, message = "Error removing from cart: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ClearCart()
        {
            try
            {
                var sessionId = GetOrCreateSessionId();
                var cartItems = _context.CartItems.Where(ci => ci.SessionId == sessionId).ToList();

                _context.CartItems.RemoveRange(cartItems);
                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    totalItems = 0,
                    subtotal = "0.00",
                    tax = "0.00",
                    total = "0.00"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ClearCart action");
                return Json(new { success = false, message = "Error clearing cart: " + ex.Message });
            }
        }

        private List<CartItem> GetCartItems()
        {
            try
            {
                var sessionId = GetOrCreateSessionId();
                var cartItems = _context.CartItems
                    .Where(ci => ci.SessionId == sessionId)
                    .Include(ci => ci.Product)
                    .ThenInclude(p => p.Category)
                    .ToList();

                return cartItems ?? new List<CartItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCartItems method");
                return new List<CartItem>();
            }
        }

        private string GetOrCreateSessionId()
        {
            try
            {
                var sessionId = HttpContext.Session.GetString("CartSessionId");
                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("CartSessionId", sessionId);
                }
                return sessionId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrCreateSessionId method");
                return Guid.NewGuid().ToString();
            }
        }

        // Fixed RenderPartialViewToString method
        private string RenderPartialViewToString(string viewName, object model)
        {
            try
            {
                using (var sw = new StringWriter())
                {
                    // Find the view using the view engine
                    var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

                    if (!viewResult.Success)
                    {
                        throw new ArgumentException($"View '{viewName}' not found");
                    }

                    // Create ViewContext with proper ViewData
                    var viewContext = new ViewContext(
                        ControllerContext,
                        viewResult.View,
                        new ViewDataDictionary(ViewData) { Model = model },
                        TempData,
                        sw,
                        new HtmlHelperOptions()
                    );

                    // Render the view - THIS IS THE FIX: Call RenderAsync on viewResult.View, not ViewData
                    var task = viewResult.View.RenderAsync(viewContext);
                    task.GetAwaiter().GetResult();

                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering partial view: {ViewName}. Error: {ErrorMessage}", viewName, ex.Message);

                // Return a more detailed fallback that includes the actual cart data
                var cartViewModel = model as CartViewModel;
                if (cartViewModel != null)
                {
                    return $@"
                <div class='cart-container'>
                    <div class='alert alert-warning'>
                        <strong>Warning:</strong> Cart display issue. You have {cartViewModel.TotalItems} item(s) in cart.
                    </div>
                    <div class='cart-totals'>
                        <p>Subtotal: {cartViewModel.Subtotal:C}</p>
                        <p>Tax: {cartViewModel.Tax:C}</p>
                        <p>Total: {cartViewModel.Total:C}</p>
                    </div>
                </div>";
                }

                return @"
            <div class='cart-container'>
                <div class='alert alert-warning'>
                    Unable to load cart contents. Please refresh the page.
                </div>
            </div>";
            }
        }

        // Alternative async version
        private async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
        {
            try
            {
                using (var sw = new StringWriter())
                {
                    // Find the view using the view engine
                    var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

                    if (!viewResult.Success)
                    {
                        throw new ArgumentException($"View '{viewName}' not found");
                    }

                    // Create ViewContext with proper ViewData
                    var viewContext = new ViewContext(
                        ControllerContext,
                        viewResult.View,
                        new ViewDataDictionary(ViewData) { Model = model },
                        TempData,
                        sw,
                        new HtmlHelperOptions()
                    );

                    // Render the view asynchronously
                    await viewResult.View.RenderAsync(viewContext);

                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering partial view: {ViewName}. Error: {ErrorMessage}", viewName, ex.Message);

                // Return fallback HTML
                var cartViewModel = model as CartViewModel;
                if (cartViewModel != null)
                {
                    return $@"
                <div class='cart-container'>
                    <div class='alert alert-warning'>
                        <strong>Warning:</strong> Cart display issue. You have {cartViewModel.TotalItems} item(s) in cart.
                    </div>
                    <div class='cart-totals'>
                        <p>Subtotal: {cartViewModel.Subtotal:C}</p>
                        <p>Tax: {cartViewModel.Tax:C}</p>
                        <p>Total: {cartViewModel.Total:C}</p>
                    </div>
                </div>";
                }

                return @"
            <div class='cart-container'>
                <div class='alert alert-warning'>
                    Unable to load cart contents. Please refresh the page.
                </div>
            </div>";
            }
        }

        public IActionResult DebugPagination()
        {
            var debugInfo = new StringBuilder();

            // Test pagination for different pages
            for (int page = 1; page <= 4; page++)
            {
                debugInfo.AppendLine($"=== PAGE {page} ===");
                var products = _productService.GetProducts(page: page, pageSize: 8);

                foreach (var product in products)
                {
                    debugInfo.AppendLine($"ID: {product.Id}, Name: {product.Name}, Created: {product.CreatedDate}");
                }
                debugInfo.AppendLine();
            }

            // Show SQL query
            debugInfo.AppendLine($"=== SQL QUERY ===");
            debugInfo.AppendLine(_productService.GetProductsQueryDebug(page: 3, pageSize: 8));

            return Content(debugInfo.ToString(), "text/plain");
        }

        // Health check endpoint for debugging
        [HttpGet]
        public JsonResult HealthCheck()
        {
            try
            {
                var dbConnected = _context.Database.CanConnect();
                var sessionId = GetOrCreateSessionId();
                var cartItemsCount = _context.CartItems.Count(ci => ci.SessionId == sessionId);

                return Json(new
                {
                    success = true,
                    databaseConnected = dbConnected,
                    sessionId = sessionId,
                    cartItemsCount = cartItemsCount,
                    serverTime = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    serverTime = DateTime.UtcNow
                });
            }
        }
    }
}