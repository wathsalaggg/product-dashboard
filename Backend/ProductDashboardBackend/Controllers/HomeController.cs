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
            var viewModel = new ProductListViewModel
            {
                Products = _productService.GetProducts(page: 1, pageSize: PageSize),
                Categories = _productService.GetCategories(),
                CurrentPage = 1,
                PageSize = PageSize
            };

            viewModel.TotalProducts = _productService.GetProductCount();
            viewModel.TotalPages = (int)Math.Ceiling((double)viewModel.TotalProducts / PageSize);

            return View(viewModel);
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
                    CategoryId = categoryId
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
                    TotalPages = 0
                });
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
    }
}