using API.Errors;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next, 
        ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        private readonly RequestDelegate next = next;
        private readonly ILogger<ExceptionMiddleware> logger = logger;
        private readonly IHostEnvironment env = env;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{message}", ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = env.IsDevelopment()
                    ? ApiException.Create(context.Response.StatusCode, ex.Message, ex.StackTrace)
                    : ApiException.Create(context.Response.StatusCode, ex.Message, "Internal Server Error");

                var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
