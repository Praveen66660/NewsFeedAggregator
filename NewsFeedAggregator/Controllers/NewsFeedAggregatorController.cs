using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewsFeedAggregator.Context;
using NewsFeedAggregator.Dtos;
using NewsFeedAggregator.Models;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NewsFeedAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsFeedAggregatorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public NewsFeedAggregatorController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserRequest request)
        {
            if (_context.Users.Any(u => u.Email == request.Email))
                return BadRequest("Email already exists.");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Interests = request.Interests
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User registered successfully.");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null || user.PasswordHash != HashPassword(request.Password))
                return Unauthorized("Invalid email or password.");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username)
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpGet("news")]
        public async Task<IActionResult> GetNews([FromQuery] string category = "general")
        {
            string apiKey = "YOUR_NEWS_API_KEY"; // get one from newsapi.org
            string url = $"https://newsapi.org/v2/top-headlines?country=us&category={category}&apiKey={apiKey}";

            using var http = new HttpClient();
            var response = await http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to fetch news");

            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }
        [Authorize]
        [HttpGet("news/personalized")]
        public async Task<IActionResult> GetNewsForUser()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);

            if (user == null || string.IsNullOrWhiteSpace(user.Interests))
                return BadRequest("User or interests not found.");

            var apiKey = _configuration["NewsApi:ApiKey"];
            var httpClient = new HttpClient();
            //tesline

            var interests = user.Interests.Split(',').Select(i => i.Trim()).ToList();
            var allArticles = new List<object>();

            foreach (var interest in interests)
            {
                var url = $"https://newsapi.org/v2/top-headlines?country=us&category={interest}&apiKey={apiKey}";
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(json);

                    if (jsonDoc.RootElement.TryGetProperty("articles", out var articles))
                    {
                        foreach (var article in articles.EnumerateArray())
                        {
                            allArticles.Add(article);
                        }
                    }
                }
            }

            return Ok(allArticles);
        }

    }
}
