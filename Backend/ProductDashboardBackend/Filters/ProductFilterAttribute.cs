using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ProductFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Custom validation or transformation logic
        if (context.ActionArguments.TryGetValue("filters", out var filtersObj)
            && filtersObj is ProductFilterParameters filters)
        {
            // Validate rating range
            if (filters.RatingGte.HasValue && filters.RatingLte.HasValue
                && filters.RatingGte > filters.RatingLte)
            {
                context.Result = new BadRequestObjectResult(
                    "Rating minimum cannot be greater than maximum");
                return;
            }

            // Set default page size limits
            filters.PageSize = Math.Min(filters.PageSize, 100); // Max 100 items per page
        }

        base.OnActionExecuting(context);
    }
}