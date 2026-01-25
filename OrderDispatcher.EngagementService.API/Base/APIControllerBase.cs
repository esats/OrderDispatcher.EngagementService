using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace OrderDispatcher.EngagementService.API.Base
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class APIControllerBase : ControllerBase
    {
        [NonAction]
        public string GetUser()
        {
            var userId = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).ToList()[0]?.Value;
            return userId;
        }
    }
}
