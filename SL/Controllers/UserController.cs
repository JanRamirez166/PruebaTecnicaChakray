using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BL.User _user;

        public UserController(BL.User user)
        {
            _user = user;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? filter, [FromQuery] string? sortedBy)
        {
            ML.Result result = _user.GetAll(filter, sortedBy);
            return StatusCode(result.status, result);
        }

    }
}
