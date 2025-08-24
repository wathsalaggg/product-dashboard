using Microsoft.AspNetCore.Mvc;
using ProductDashboard.Models.ViewModels;
using ProductDashboard.Services;
using ProductDashboardBackend.Data;

namespace ProductDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productService;

        public HomeController(ApplicationDbContext context)
        {
            _productService = new ProductService(context);
        }

        public IActionResult Index()
        {
            var viewModel = new ProductListViewModel
            {
                Products = _productService.GetProducts(page: 1, pageSize: 9),
                Categories = _productService.GetCategories(),
                CurrentPage = 1,
                PageSize = 9
            };

            viewModel.TotalProducts = _productService.GetProductCount();
            viewModel.TotalPages = (int)Math.Ceiling((double)viewModel.TotalProducts / viewModel.PageSize);

            return View(viewModel);
        }
    }
}