using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Attributes;

namespace WebApplication5.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [RevockToken("RevockToken")]
    [AccessDenied("AccessDenied")]
    public class BaseController : Controller{}
}
