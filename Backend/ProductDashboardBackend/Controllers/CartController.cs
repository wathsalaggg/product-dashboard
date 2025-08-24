using Microsoft.AspNetCore.Mvc;
using ProductDashboardBackend.Services;
using ProductDashboard.Models;
using ProductDashboard.Models.ViewModels;
using ProductDashboardBackend.Data;
using ProductDashboard.Services;

namespace ProductDashboard.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductService _productService;
        private const string CartSessionKey = "ShoppingCart";

        // Inject ApplicationDbContext via constructor
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
            _productService = new ProductService(_context);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _productService.GetProduct(productId);
            if (product == null || !product.InStock)
            {
                return Json(new { success = false, message = "Product not found or out of stock" });
            }

            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    Product = product,
                    Quantity = quantity
                });
            }

            SaveCartToSession(cart);

            return Json(new
            {
                success = true,
                message = "Product added to cart",
                cartCount = cart.Sum(x => x.Quantity)
            });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCartToSession(cart);
            }

            return Json(new
            {
                success = true,
                message = "Product removed from cart",
                cartCount = cart.Sum(x => x.Quantity)
            });
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            var cart = GetCartFromSession();
            var viewModel = new CartViewModel
            {
                Items = cart
            };

            return Json(new
            {
                success = true,
                cart = new
                {
                    items = cart.Select(x => new
                    {
                        productId = x.ProductId,
                        name = x.Product.Name,
                        price = x.Product.Price,
                        quantity = x.Quantity,
                        totalPrice = x.TotalPrice,
                        imageUrl = x.Product.ImageUrl
                    }),
                    totalAmount = viewModel.TotalAmount,
                    totalItems = viewModel.TotalItems
                }
            });
        }

        private List<CartItem> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }

            var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson);

            // Reload product data
            foreach (var item in cartItems)
            {
                item.Product = _productService.GetProduct(item.ProductId);
            }

            return cartItems;
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            var cartJson = System.Text.Json.JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }
}