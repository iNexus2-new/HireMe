using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace HireMe.Core.Helpers
{
	public class FlagsEnumBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			object result;
			var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			if (value == null) return Task.FromResult(ModelBindingResult.Failed());

			if (value.Values.Count > 0)
				result = Enum.Parse(bindingContext.ModelType, value.Values);
			else
				result = Activator.CreateInstance(bindingContext.ModelType);

			bindingContext.Result = ModelBindingResult.Success(result);
			return Task.CompletedTask;
		}
	}
	public class FlagsEnumBinderProvider : IModelBinderProvider
	{
		public IModelBinder GetBinder(ModelBinderProviderContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			if (context.Metadata.IsFlagsEnum)
				return new FlagsEnumBinder();

			return null;
		}
	}
}
