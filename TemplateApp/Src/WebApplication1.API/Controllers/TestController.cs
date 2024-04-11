using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TestController : ControllerBase
    {
        // GET: TestController
        [HttpGet]
        public IActionResult Get()
        {
            return this.Ok("Test endoint");
        }
    }
}
