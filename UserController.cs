using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private static List<User> Users = new List<User>();

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if (Users.Any(u => u.Username == user.Username))
                return BadRequest("User already exists");

            Users.Add(user);
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login(User user)
        {
            var existingUser = Users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
            if (existingUser == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Add_Your_Secret_Security_Key_Here");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
