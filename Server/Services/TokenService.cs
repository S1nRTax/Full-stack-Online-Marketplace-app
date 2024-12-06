using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Models;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Server.Services
{
    public class TokenService : ITokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration; 
        private readonly ILogger<TokenService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(
            ApplicationDbContext context,
            UserManager<User> userManager, 
            IConfiguration configuration, 
            ILogger<TokenService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            var key = Encoding.UTF8.GetBytes(_configuration["AppSettings:JWTSecret"] ?? throw new InvalidOperationException("JWT Secret not configured"));
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }


        public string GenerateRefreshToken(int size = 64)
        {
            byte[] randomBytes = new byte[size];
            // fill the byte array with cryptographically secure random bytes
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes)
                      .Replace("+", "-")
                      .Replace('?', '-')
                      .Replace("/", "_")
                      .TrimEnd('=');     // Remove padding for URL safety

        }


        public async Task<string> RetrieveUseridByRefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                // You can return a custom error message or throw an exception if the refreshToken is invalid.
                throw new ArgumentException("Refresh token cannot be null or empty.", nameof(refreshToken));
            }

            try
            {
                var tokenEntry = await _context.RefreshTokens
                                               .FirstOrDefaultAsync(t => t.Token == refreshToken);

                if (tokenEntry == null)
                {
                    return "Token not found.";
                }


                return tokenEntry.UserId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving userId by refresh token.");
                throw new InvalidOperationException("An error occurred while retrieving the user ID.", ex);
            }
        }


        public async Task SaveRefreshTokenAsync(User user, RefreshToken refreshToken)
        { 
            // Find existing token for the user
            var existingToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t =>
                    t.UserId == user.Id &&
                    t.Token == refreshToken.Token);

            // Set consistent expiration and creation times
            refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
            refreshToken.CreateAt = DateTime.UtcNow;
            refreshToken.UserId = user.Id;

            if (existingToken != null)
            {
                // Update existing token
                _context.Entry(existingToken).CurrentValues.SetValues(refreshToken);
            }
            else
            {
                // Add new token
                await _context.RefreshTokens.AddAsync(refreshToken);
            }

            await _context.SaveChangesAsync();
        }



        public  void SetRefreshToken(RefreshToken refreshToken, User user)
        {

            if (_httpContextAccessor.HttpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            _httpContextAccessor.HttpContext.Response.Cookies.Append("refresh_token", refreshToken.Token,
                new CookieOptions
                {
                    Expires = refreshToken.ExpiresAt,
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
        }


    



    }
}
