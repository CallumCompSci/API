using Microsoft.AspNetCore.Mvc;
using Interfaces;
using Last_Api.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace Last_Api.Contollers
{
    [ApiController]
    [Route("api/auth")]
    [Tags("Login Endpoint:")]
    public class JWTContoller : ControllerBase
    {
        private readonly IUser _UserRepo;

        public JWTContoller(IUser UserRepo)
        {
            _UserRepo = UserRepo;
        }

        [HttpPost("login")]
        [EndpointName("Get token")]
        [EndpointSummary("Get token")]
        [EndpointDescription(@"Verifies login credentials and issues token
        Body should look like:
        {
        ""email"": ""EMAIL@EMAIL.COM"",
        ""password"": ""PASSWORD"",
        }
        ")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Artifact>>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] Login _login)
        {
            User user = _UserRepo.GetUser(_login.email);
            if (user == null) { return NotFound($"No user with the email: {_login.email} found"); }
            if (validUser(user, _login))
            {
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }
            return Unauthorized("Login details incorrect");
        }


        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("4b068b45b1614236b6052beee7e689c14f2180c122ff41d3a17778d49ef8283e"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.role)
            };

            var token = new JwtSecurityToken(
                issuer: "CallumsAPI",
                audience: "APIUser",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool validUser(User user, Login _login)
        {
            if (BCrypt.Net.BCrypt.EnhancedVerify(_login.password, user.password))
            {
                return true;
            }
            else
            {
                return false;
            }

        }



    }
}
