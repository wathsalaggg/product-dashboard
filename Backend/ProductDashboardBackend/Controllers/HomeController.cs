using Microsoft.AspNetCore.Mvc;
using ProductDashboard.Models;
using ProductDashboard.Models.ViewModels;
using ProductDashboard.Services;
using ProductDashboardBackend.Data;
using System.Text;

namespace ProductDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productService;
        private const int PageSize = 8; // Consistent page size

        public HomeController(ApplicationDbContext context)
        {
            _productService = new ProductService(context);
        }

        public IActionResult Index()
        {
            // Return the main view without loading products initially
            // Products will be loaded via AJAX when user navigates to Products
            return View();
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
                    Categories = _productService.GetCategories() // Add categories for the filter dropdown
                };

                return PartialView("~/Views/Product/_ProductList.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProductList: {ex.Message}");

                return PartialView("~/Views/Product/_ProductList.cshtml", new ProductListViewModel
                {
                    Products = new List<Product>(),
                    CurrentPage = 1,
                    TotalProducts = 0,
                    TotalPages = 0,
                    Categories = _productService.GetCategories()
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
    }
}