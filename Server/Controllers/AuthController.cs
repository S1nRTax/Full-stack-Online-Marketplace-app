using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger _logger;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context,
            ITokenService tokenService,
            ILogger<AuthController> logger)
            
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // this checks if the username is already taken
                var existingUserByUsername = await _userManager.FindByNameAsync(model.Username);
                if (existingUserByUsername != null)
                {
                    ModelState.AddModelError("Username", "Username is already taken.");
                    return BadRequest(ModelState);
                }
                // this checks if the Email is already taken
                var existingUserByEmail = await _userManager.FindByNameAsync(model.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Email", "Email is already taken.");
                    return BadRequest(ModelState);
                }

                // Then we create the user
                var user = new User
                {
                    UserName = model.Username ?? model.Email,
                    Name = model.Name,
                    Email = model.Email,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = false
                };
                // hashing password
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        Message = "User creation failed",
                        Errors = result.Errors.Select(e => e.Description)
                    });
                }


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // save and commit the changes:
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    Message = "Registration successful",
                    User = new { user.Id, user.UserName , user.Name, user.Email}
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "An error occurred during registration", Error = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Find user by email
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                    return Unauthorized(new { Message = "Invalid email or password." });

                // Generate access token and set as cookie
                var accessToken = _tokenService.GenerateAccessToken(user);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None, // Ensure CSRF protection elsewhere
                    Expires = DateTime.UtcNow.AddMinutes(1)
                };
                HttpContext.Response.Cookies.Append("access_token", accessToken, cookieOptions);

                // Generate and save refresh token
                var refreshToken = new RefreshToken
                {
                    Token = _tokenService.GenerateRefreshToken(),
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreateAt = DateTime.UtcNow
                };

                _tokenService.SetRefreshToken(refreshToken, user);
                await _tokenService.SaveRefreshTokenAsync(user , refreshToken);

                await _context.SaveChangesAsync();
                // Return success response
                return Ok(new
                {
                    Message = "Login successful",
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.UserName
                });


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, new { Message = "An error occurred during login", Error = ex.Message });
            }
        }



        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refresh_token"];
                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized(new { Message = "Refresh token is missing." });

                // Fetch refresh token from the database
                var tokenEntry = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
                if (tokenEntry == null || tokenEntry.ExpiresAt < DateTime.UtcNow)
                    return Unauthorized(new { Message = "Invalid or expired refresh token." });

                // Find user associated with refresh token
                var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == tokenEntry.UserId);
                if (user == null)
                    return Unauthorized(new { Message = "User not found." });

                // Generate new tokens
                var newAccessToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                // Save the new refresh token in the database
                tokenEntry.Token = newRefreshToken;
                tokenEntry.ExpiresAt = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                // Set the new refresh token as a cookie
                Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during token refresh.");
                return StatusCode(500, new { Message = "An error occurred during token refresh", Error = ex.Message });
            }
        }



        [HttpDelete]
        public async Task RevokeTokenAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Remove all refresh tokens for the specified user
            var userTokens = await _context.RefreshTokens
                .Where(t => t.UserId == user.Id)
                .ToListAsync();

            if (userTokens.Count != 0)
            {
                _context.RefreshTokens.RemoveRange(userTokens);
                await _context.SaveChangesAsync();
            }
        }




        [Authorize] // Ensures the token is validated
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new
            { 
                id = user.Id,
                username = user.UserName,
                email = user.Email
            });
        }




        [HttpPost("logout")]
        [Authorize] // Ensure that only authenticated users can log out
        public IActionResult Logout()
        {
            // Delete the access token cookie
            Response.Cookies.Delete("access_token");

            // Sign out from ASP.NET Identity
            _signInManager.SignOutAsync();

            return Ok(new { Message = "Logged out successfully!" });
        }


    }
}