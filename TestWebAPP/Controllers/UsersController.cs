using Microsoft.AspNetCore.Mvc;

namespace TestWebAPP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetUser()
        {
            return Ok(new
            {
                Message = "Hello from my first app",
                Status = "Success"
            });
        }
    }
}