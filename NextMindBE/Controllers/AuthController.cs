using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NextMindBE.Data;
using NextMindBE.DTOs;
using NextMindBE.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NextMindBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _configuration { get; }
        public ApplicationDbContext _context { get; }

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = _context.User.SingleOrDefault(o => o.Username == request.Username);
            if (user != null)
            {
                return BadRequest("Username already existing.");
            }
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var newUser = new User()
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                SessionId = string.Empty,
            };

            _context.Add(newUser);
            _context.SaveChangesAsync();
            return Ok(newUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var user = _context.User.SingleOrDefault(o => o.Username == request.Username);
            if(user == null)
            {
                return BadRequest("Something went wrong");
            }

            if(!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Something went wrong.");
            }
            var guid = Guid.NewGuid().ToString();
            user.LastActive = DateTime.UtcNow.AddSeconds(20);
            user.SessionId = guid;
            PingTimerManager._authenticatedUsers.Add(guid, user);
            var val = new Dictionary<string, string>()
            {
                { "token", CreateToken(user) },
                { "sessionId" , guid },
            };

            _context.User.Update(user);

            _context.SaveChangesAsync();
            return Ok(JsonConvert.SerializeObject(val));
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials:cred
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passowrdSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passowrdSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passowrdSalt)
        {
            using(var hmac = new HMACSHA512(passowrdSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

    }
}
