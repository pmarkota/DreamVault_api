using DreamVault_API.Models;
using DreamVault_API.Requests.AppUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DreamVault_API.Controllers
{

    public class GetByToken
    {
        public string Token { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly PostgresContext _db;
        private readonly IConfiguration _config;

        public AppUserController(PostgresContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpGet]
        public ActionResult<AppUser> Get()
        {
            return Ok(_db.AppUsers);
        }

        [HttpPost("register")]
        public ActionResult<AppUser> Register(RegisterRequest request)
        {
            var existingUserByEmail = _db.AppUsers.FirstOrDefault(u => u.Email == request.Email);
            var existingUserByUsername = _db.AppUsers.FirstOrDefault(u => u.Username == request.Username);

            if (existingUserByEmail != null)
            {
                return BadRequest(new { message = "Email already taken" });
            }

            if (existingUserByUsername != null)
            {
                return BadRequest(new { message = "Username already taken" });
            }

            // Hash the password using BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var appUser = new AppUser
            {
                Email = request.Email,
                Username = request.Username,
                PasswordHash = hashedPassword,
                SubscriptionPlanId = 1,
                SubscriptionExpirationDate = DateTime.MaxValue,
                AiImagesGeneratedThisMonth = 0,
                LastResetDate = DateTime.UtcNow
            };

            _db.AppUsers.Add(appUser);
            _db.SaveChanges();
            var token = GenerateJwtToken(appUser);

            return Ok(new { message = "User registered successfully", token });
        }


        [HttpPost("login")]
        public ActionResult<AppUser> Login(LoginRequest request)
        {
            var user = _db.AppUsers.FirstOrDefault(u => u.Username == request.Username);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid credentials" });
            }

            // Verify the password
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                return BadRequest(new { message = "Invalid credentials" });
            }

            var token = GenerateJwtToken(user);

            return Ok(new { message = "User logged in successfully", token });
        }

        //get a user by token, there is a custom claim in each token "id" so get the user by that token, token will be from body
        [HttpPost("getuserbytoken")]
        public ActionResult<AppUser> GetUserByToken([FromBody] GetByToken token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("Jwt:Key"));
            tokenHandler.ValidateToken(token.Token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false,
                ValidateAudience = false,
                ValidateIssuer = false
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            var user = _db.AppUsers.FirstOrDefault(u => u.AppUserId == userId);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid token" });
            }

            return Ok(new
            {
                user.Username,
                user.SubscriptionPlanId,
                user.SubscriptionExpirationDate,
                user.AiImagesGeneratedThisMonth,
                user.LastResetDate
            });
        }

        private string GenerateJwtToken(AppUser user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:Key"))); // Replace with your secret key
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim("id", user.AppUserId.ToString()), // Assuming 'Id' is a valid property in your 'AppUser' class
        new Claim("username", user.Username), // Assuming 'Id' is a valid property in your 'AppUser' class
        new Claim(ClaimTypes.Email, user.Email),
        // Add any additional claims here
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMonths(100), // Token expiration time
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
