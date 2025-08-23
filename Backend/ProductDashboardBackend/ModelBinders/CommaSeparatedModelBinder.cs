using Microsoft.AspNetCore.Mvc.ModelBinding;

public class CommaSeparatedModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelType != typeof(List<int>) &&
            bindingContext.ModelType != typeof(List<int>))
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

        if (string.IsNullOrEmpty(value))
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        var integers = value.Split(',')
            .Where(x => int.TryParse(x.Trim(), out _))
            .Select(x => int.Parse(x.Trim()))
            .ToList();

        bindingContext.Result = ModelBindingResult.Success(integers);
        return Task.CompletedTask;
    }
}