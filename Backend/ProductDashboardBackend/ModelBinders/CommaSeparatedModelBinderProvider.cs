using Microsoft.AspNetCore.Mvc.ModelBinding;

public class CommaSeparatedModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(List<int>) ||
            context.Metadata.ModelType == typeof(List<int>))
        {
            return new CommaSeparatedModelBinder();
        }
        return null;
    }
}