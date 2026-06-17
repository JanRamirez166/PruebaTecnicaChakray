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
        [HttpGet]
        [AllowAnonymous]
        [Route("ObtenerToken")]
        public IActionResult GenerarToken()
        {
            string token = GenerarJwtToken();
            return Ok(token);
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
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
