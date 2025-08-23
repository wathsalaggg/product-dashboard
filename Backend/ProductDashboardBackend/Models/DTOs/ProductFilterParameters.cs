using System.ComponentModel.DataAnnotations;

public class ProductFilterParameters
{
    public string? S { get; set; } // Search term
    public List<int>? CategoryIds { get; set; }

    [Range(0, 5)]
    public decimal? RatingGte { get; set; } // Rating greater than or equal

    [Range(0, 5)]
    public decimal? RatingLte { get; set; } // Rating less than or equal

    [Range(0, int.MaxValue)]
    public int? QuantityGte { get; set; } // Quantity greater than or equal

    // Pagination parameters
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}