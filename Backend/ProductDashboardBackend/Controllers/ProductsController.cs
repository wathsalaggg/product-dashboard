using Microsoft.AspNetCore.Mvc;
using ProductDashboardBackend.Services;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<Product>>> GetProducts(
        [FromQuery] ProductFilterParameters filters)
    {
        try
        {
            var result = await _productService.GetFilteredProductsAsync(filters);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving products." });
        }
    }
}