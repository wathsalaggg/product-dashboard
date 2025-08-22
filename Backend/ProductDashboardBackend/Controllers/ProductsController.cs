using Microsoft.AspNetCore.Mvc;

namespace ProductDashboardBackend.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
