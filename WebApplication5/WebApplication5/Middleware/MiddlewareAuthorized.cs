using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using WebApplication5.Attributes;
using WebApplication5.Authentication;
using WebApplication5.Redis;

namespace WebApplication5.Middleware
{
    public class MiddlewareAuthorized
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<MiddlewareAuthorized> _logger;
        private readonly IRedisCacheService redisCacheService;
        public MiddlewareAuthorized(RequestDelegate _requestDelegate, ILogger<MiddlewareAuthorized> _logger, IRedisCacheService redisCacheService)
        {
            this._requestDelegate = _requestDelegate;
            this._logger = _logger;
            this.redisCacheService = redisCacheService;
        }
        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("MyMiddleware executing..");

            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<AccessDeniedAttribute>();

            _logger.LogInformation("attribute : " + attribute);

            if (attribute != null && !string.IsNullOrEmpty(attribute.accessDenied))
            {
                IEnumerable<Claim> claims = context.User.Claims;
                if (claims != null && claims.Any())
                {
                    AccessDeniedDTO authorized = JsonConvert.DeserializeObject<AccessDeniedDTO>(claims.First(x => x.Type == "authorized").Value);

                    string [] path = context.Request.Path.Value.Split('/');
                    
                    if (authorized != null && !authorized.Action.Equals(path[path.Length-1]))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            Data = "",
                            ErrorCode = 500,
                            ErrorMessage = "access denied url",
                        });
                        return;
                    }
                }
            }
            await _requestDelegate(context);
        }
    }
}
