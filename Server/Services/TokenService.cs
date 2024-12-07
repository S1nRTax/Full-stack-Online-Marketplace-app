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
                var tokenEntry = await _context.AccessTokens
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



        public async Task SaveAccessTokenAsync(User user, string token)
        {
            // Create a new AccessToken instance
            var accessToken = new AccessToken
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreateAt = DateTime.UtcNow,
                UserId = user.Id
            };

            // Check if a token with the same value already exists
            var existingToken = await _context.AccessTokens
                .AsNoTracking() // Avoid EF tracking conflicts
                .FirstOrDefaultAsync(t => t.Token == token);

            if (existingToken != null)
            {
                // Detach the existing entity to prevent tracking conflicts
                _context.Entry(existingToken).State = EntityState.Detached;
                _context.AccessTokens.Update(accessToken);
            }
            else
            {
                // Remove old tokens for the user
                var oldTokens = _context.AccessTokens.Where(t => t.UserId == user.Id);
                _context.AccessTokens.RemoveRange(oldTokens);

                // Add the new token
                await _context.AccessTokens.AddAsync(accessToken);
            }

            // Save changes
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the error for debugging
                Console.Error.WriteLine($"Error saving access token: {ex.Message}");
                throw;
            }
        }






        public void SetAccessToken(AccessToken accessToken, User user)
        {

            if (_httpContextAccessor.HttpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            _httpContextAccessor.HttpContext.Response.Cookies.Append("refresh_token", accessToken.Token,
                new CookieOptions
                {
                    Expires = accessToken.ExpiresAt,
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
        }


    



    }
}
