namespace ProductDashboardBackend.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }   
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string IconName { get; set; } = null!;

        // Navigation property
        public List<Product> Products { get; set; } = new();
    }
}
