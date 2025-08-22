using Microsoft.AspNetCore.Mvc;

namespace ProductDashboardBackend.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
