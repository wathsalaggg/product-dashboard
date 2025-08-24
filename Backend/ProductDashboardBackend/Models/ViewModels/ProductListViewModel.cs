using ProductDashboard.Models;

namespace ProductDashboard.Models.ViewModels
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalProducts { get; set; }
        public int PageSize { get; set; } = 8;
        public string SearchTerm { get; set; }
        public int? CategoryId { get; set; }
    }
}