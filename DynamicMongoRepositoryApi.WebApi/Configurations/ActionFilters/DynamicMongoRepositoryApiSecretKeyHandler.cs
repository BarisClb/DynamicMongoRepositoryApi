using DynamicMongoRepositoryApi.WebApi.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace DynamicMongoRepositoryApi.WebApi.Configurations.ActionFilters
{
    public class DynamicMongoRepositoryApiSecretKeyHandler : ActionFilterAttribute, IActionFilter
    {
        private readonly IOptions<DynamicMongoRepositoryApiSettings> _dynamicMongoRepositoryApiSettings;

        public DynamicMongoRepositoryApiSecretKeyHandler(IOptions<DynamicMongoRepositoryApiSettings> dynamicMongoRepositoryApiSettings)
        {
            _dynamicMongoRepositoryApiSettings = dynamicMongoRepositoryApiSettings ?? throw new ArgumentNullException(nameof(dynamicMongoRepositoryApiSettings));
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            if (string.IsNullOrEmpty(request.Headers["api-key"]))
            {
                filterContext.Result = new UnauthorizedObjectResult("Please provide the Api-Key.");
                return;
            }

            var apiKey = AuthenticationHeaderValue.Parse(request.Headers["api-key"]).ToString();
            if (!string.Equals(apiKey, _dynamicMongoRepositoryApiSettings.Value.ApiSecretKey, StringComparison.OrdinalIgnoreCase))
                filterContext.Result = new UnauthorizedObjectResult("Incorrect Api-Key.");
        }
    }
}
