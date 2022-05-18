using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class)] // indicates where the api key will be used, in this case all controllers/classes
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyName = "key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyName, out Microsoft.Extensions.Primitives.StringValues ExtractedApiKey)) // check if request headers contain api key
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "missing api key"
                };

                return;
            }

            IConfiguration AppSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            string ApiKey = AppSettings.GetValue<string>(ApiKeyName);

            if (!ApiKey.Equals(ExtractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "api key not valid"
                };

                return;
            }

            await next();
        }
    }
}
