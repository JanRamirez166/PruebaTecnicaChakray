using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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

        [HttpPost]
        public IActionResult Add([FromBody] ML.User user)
        {
            ML.Result result = _user.Add(user);
            return StatusCode(result.status, result);
        }

        [HttpPatch("{id}")]
        public IActionResult PartialUpdate([FromBody] JsonPatchDocument<ML.User> user, string id)
        {
            //ML.Result result = _user.PartialUpdate(user, id);
            ML.Result result = _user.GetById(id);
            ML.User userFind = (ML.User)result.Object;

            if (result.Correct) {
                user.ApplyTo(userFind);
            }
            return StatusCode(result.status, userFind);
        }

    }
}
