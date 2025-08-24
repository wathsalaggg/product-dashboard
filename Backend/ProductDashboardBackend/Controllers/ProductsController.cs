//using Microsoft.AspNetCore.Mvc;
//using ProductDashboardBackend.Services;

//[ApiController]
//[Route("api/[controller]")]
//public class ProductsController : ControllerBase
//{
//    private readonly IProductService _productService;

//    public ProductsController(IProductService productService)
//    {
//        _productService = productService;
//    }

//    [HttpGet]
//    public async Task<ActionResult<PagedResult<Product>>> GetProducts(
//        [FromQuery] ProductFilterParameters filters)
//    {
//        try
//        {
//            var result = await _productService.GetFilteredProductsAsync(filters);
//            return Ok(result);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new { message = "An error occurred while retrieving products." });
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using ProductDashboard.Models.ViewModels;
using ProductDashboard.Services;
using ProductDashboardBackend.Data;

namespace ProductDashboard.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        public ProductController(ApplicationDbContext context)
        {
            _productService = new ProductService(context);
        }

        [HttpGet]
        public IActionResult GetProducts(string searchTerm, int? categoryId, int page = 1, int pageSize = 9)
        {
            var products = _productService.GetProducts(searchTerm, categoryId, page, pageSize);
            var totalCount = _productService.GetProductCount(searchTerm, categoryId);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var viewModel = new ProductListViewModel
            {
                Products = products,
                Categories = _productService.GetCategories(),
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                CurrentPage = page,
                PageSize = pageSize,
                TotalProducts = totalCount,
                TotalPages = totalPages
            };

            return PartialView("_ProductList", viewModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var product = _productService.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            return Json(new
            {
                success = true,
                product = new
                {
                    id = product.Id,
                    name = product.Name,
                    description = product.Description,
                    price = product.Price,
                    imageUrl = product.ImageUrl,
                    category = product.Category?.Name,
                    inStock = product.InStock
                }
            });
        }
    }
}