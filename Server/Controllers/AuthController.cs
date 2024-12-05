using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Server.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;

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

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
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
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                    return BadRequest(new { Message = "Invalid email or password." });

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                        
                    // Create claims for the 
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Email, user.Email!),
                        new Claim(ClaimTypes.Name, user.UserName!)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_configuration["AppSettings:JWTSecret"] ?? throw new InvalidOperationException("JWT Secret not configured"));

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature)
                    };


                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    // Set the token as an HttpOnly cookie
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true, // Prevent access via JavaScript
                        Secure = true, // Use HTTPS in production
                        SameSite = SameSiteMode.None, // Allow cross-site requests
                        Expires = DateTime.UtcNow.AddDays(7) // Match JWT expiration
                    };

                    HttpContext.Response.Cookies.Append("access_token", tokenString, cookieOptions);
                    


                    return Ok(new { Token = tokenString, Message = "Login successful", UserId = user.Id, Email = user.Email, Username = user.UserName });
                }

                if (result.IsLockedOut)
                    return BadRequest(new { Message = "Account is locked out." });

                return BadRequest(new { Message = "Invalid login attempt." });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { Message = "An error occurred during login", Error = ex.Message });
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