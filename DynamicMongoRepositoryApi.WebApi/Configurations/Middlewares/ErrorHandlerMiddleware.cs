using DynamicMongoRepositoryApi.WebApi.Helpers;
using DynamicMongoRepositoryApi.WebApi.Models.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using System.Text;

namespace DynamicMongoRepositoryApi.WebApi.Configurations.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }


        public async Task Invoke(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                await _next(context);
            }
            catch (Exception exception)
            {
                await ExceptionHandler(context, exception);
            }
        }

        private async Task ExceptionHandler(HttpContext context, Exception exception)
        {
            string request = await GetRequestBody(context.Request);

            Console.WriteLine(exception);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResponse<NoContent>() { Data = null, IsSuccess = false, StatusCode = 500, Error = new() { ApiErrorMessage = GenericDataHelper.ErrorHandlerMiddlewareApiError, Exception = exception.ToString(), ExceptionMessage = exception.Message, RequestBody = request } }));
        }

        private async Task<string> GetRequestBody(HttpRequest request)
        {
            try
            {
                request.Body.Seek(0, SeekOrigin.Begin);
                using StreamReader reader = new StreamReader(request.Body, Encoding.UTF8);
                return await reader.ReadToEndAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return "";
            }
        }
    }
}
