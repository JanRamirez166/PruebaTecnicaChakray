using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly BL.Login _login;

        public LoginController(BL.Login login)
        {
            _login = login;
        }

        [HttpPost]
        public IActionResult Login([FromBody] ML.Login login)
        {
            //Primero encontrar a el usuario por id
            ML.Result result = _login.Logearse(login);

            if (result.Correct)
            {
                string token = GenerarJwtToken();
                return Ok(token);
            }
            else
            {
                return StatusCode(result.status, result);
            }

            return StatusCode(result.status, result);


        }

        [NonAction]
        public string GenerarJwtToken()
        {
            var claims = new[] {
                new Claim(ClaimTypes.Role, "Administrador"),
                new Claim(ClaimTypes.Name, "Elios")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ddlkfhlwuefefgksdfdsfheoiyeoyyuefk"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
