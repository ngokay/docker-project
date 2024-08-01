using Microsoft.AspNetCore.Http.Features;
using System.Net;
using System.Security.Claims;
using WebApplication5.Attributes;
using WebApplication5.Redis;

namespace WebApplication5.Middleware
{
    public class MiddlewareCheckRevockToken
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<MiddlewareCheckRevockToken> _logger;
        private readonly IRedisCacheService redisCacheService;
        public MiddlewareCheckRevockToken(RequestDelegate _requestDelegate, ILogger<MiddlewareCheckRevockToken> _logger, IRedisCacheService redisCacheService)
        {
            this._requestDelegate = _requestDelegate;
            this._logger = _logger;
            this.redisCacheService = redisCacheService;
        }
        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("MyMiddleware executing..");

            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<RevockTokenAttribute>();

            _logger.LogInformation("attribute : " + attribute);

            if (attribute != null && attribute.revockToken.Equals("RevockToken"))
            {
                IEnumerable<Claim> claims = context.User.Claims;
                if (claims != null && claims.Any())
                {
                    string[] authHeader = context.Request.Headers["Authorization"].ToString().Split(" ");
                    string userName = claims.First(x=>x.Type == "UserName").Value;

                    string keyRedis = string.Format("{0}.{1}", userName, "revockToken");
                    string tokenRevock = redisCacheService.GetData<string>(keyRedis);
                    if (!String.IsNullOrEmpty(tokenRevock) && authHeader[1].Equals(tokenRevock))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            Data = 1,
                            Env = "Revock token",
                        });
                        return;
                    }
                }
            }
            await _requestDelegate(context);
        }
    }
}
