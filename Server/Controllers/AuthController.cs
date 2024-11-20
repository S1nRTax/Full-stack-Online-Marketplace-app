using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// this is an authentication controller that uses Identity core.

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if email already exists
                var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingUserByEmail != null)
                {
                    return BadRequest(new { Message = "Email is already registered." });
                }

                // Check if username already exists
                var existingUserByName = await _userManager.FindByNameAsync(model.Username);
                if (existingUserByName != null)
                {
                    return BadRequest(new { Message = "Username is already taken." });
                }

                var user = new IdentityUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    EmailConfirmed = true  // Add this if you don't want to require email confirmation
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "Registration successful" });
                }

                // If we get here, something failed
                return BadRequest(new { Message = "Registration failed", Errors = result.Errors });
            }
            catch (Exception ex)
            {
                // Log the exception
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
        if (user == null)
        {
            return BadRequest(new { Message = "Invalid email or password." });
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordValid)
        {
            return BadRequest(new { Message = "Invalid email or password." });
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (result.Succeeded)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["AppSettings:JWTSecret"] ?? 
                throw new InvalidOperationException("JWT Secret not configured"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.Name, user.UserName!)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Token = tokenString,
                Message = "Login successful",
                UserId = user.Id,
                Email = user.Email,
                Username = user.UserName
            });
        }

        if (result.IsLockedOut)
        {
            return BadRequest(new { Message = "Account is locked out." });
        }

        if (result.RequiresTwoFactor)
        {
            return BadRequest(new { Message = "Two factor authentication required." });
        }

        return BadRequest(new { Message = "Invalid login attempt." });
    }
    catch (Exception ex)
    {
        // Log the exception
        return StatusCode(500, new { Message = "An error occurred during login", Error = ex.Message });
    }
}
    }
}