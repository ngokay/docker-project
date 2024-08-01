
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using System.Net;
using WebApplication5.Attributes;

namespace WebApplication5.Middleware
{
    public class MiddlewareExcusion
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MiddlewareExcusion> _logger;

        public MiddlewareExcusion(RequestDelegate next, ILogger<MiddlewareExcusion> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("MyMiddleware executing..");

            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<ExcusionAttribute>();

            _logger.LogInformation("attribute : " + attribute);
            if (attribute != null)
            {
                string data = attribute._attribute;
                if(!String.IsNullOrEmpty(data) && data.Equals("Test"))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Data = 1,
                        Env = "ddd",
                    });
                    return;
                }
                
            }
            await _next(context); // calling next middleware
        }
    }
}
