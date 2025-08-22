using Microsoft.AspNetCore.Mvc;

namespace ProductDashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // Sample in-memory data
        private static readonly List<string> Products = new List<string>
        {
            "Pizza", "Burger", "Pasta"
        };

        // GET api/products
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(Products);
        }

        // GET api/products/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (id < 0 || id >= Products.Count)
                return NotFound();

            return Ok(Products[id]);
        }

        // POST api/products
        [HttpPost]
        public IActionResult Add([FromBody] string product)
        {
            Products.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = Products.Count - 1 }, product);
        }
    }
}
