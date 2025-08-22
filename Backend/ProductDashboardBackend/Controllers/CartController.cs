using Microsoft.AspNetCore.Mvc;

namespace ProductDashboardBackend.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
