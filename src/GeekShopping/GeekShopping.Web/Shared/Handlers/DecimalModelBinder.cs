using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace GeekShopping.Web.Shared.Handlers
{
    public class DecimalModelBinder : IModelBinder
    {
        /// <summary>
        /// Asynchronously binds a model within a specific binding context. This implementation tries to parse a decimal value from the provided input,
        /// replacing period (.) with comma (,) to adapt to culture-specific number formatting.
        /// </summary>
        /// <param name="bindingContext">
        /// The context for model binding, containing details such as the model name, the value provider, and the model state.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous model binding operation. If successful, a decimal value is bound to the model; otherwise, errors are
        /// reported in the model state.
        /// </returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if(valueProviderResult.Equals(ValueProviderResult.None)) return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            string? value = valueProviderResult.FirstValue;

            if(string.IsNullOrEmpty(value)) return Task.CompletedTask;

            value = value.Replace(".", ",");

            if(!decimal.TryParse(value, NumberStyles.Any, new CultureInfo("pt-BR"), out decimal result))
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid number format!");
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}