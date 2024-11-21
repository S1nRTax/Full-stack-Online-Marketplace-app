using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
            // Check if the model is valid using data annotations
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Create the user
                var user = new User
                {
                    UserName = model.Username ?? model.Email, // Fallback to email if username is not provided
                    Name = model.Name,
                    Email = model.Email,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = false
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                // Check if user creation was successful
                if (!result.Succeeded)
                {
                    return BadRequest(new { Message = "Registration failed", Errors = result.Errors });
                }

                // Create vendor or customer based on user type
                if (model.UserType == "Vendor")
                {
                    var vendor = new Vendor
                    {
                        ShopId = model.ShopId!,
                        ShopName = model.ShopName!,
                        ShopAddress = model.ShopAddress!,
                        ShopLogo = model.ShopLogo,
                        Popularity = 0,
                        UserId = user.Id
                    };
                    await _context.Vendors.AddAsync(vendor);
                }
                else if (model.UserType == "Customer")
                {
                    var customer = new Customer
                    {
                        Username = model.Username!,
                        ProfilePicture = model.ProfilePicture,
                        UserId = user.Id
                    };
                    await _context.Customers.AddAsync(customer);
                }

                await _context.SaveChangesAsync();

                return Ok(new { Message = "Registration successful" });
            }
            catch (Exception ex)
            {
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
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_configuration["AppSettings:JWTSecret"] ?? throw new InvalidOperationException("JWT Secret not configured"));

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

                    return Ok(new { Token = tokenString, Message = "Login successful", UserId = user.Id, Email = user.Email, Username = user.UserName });
                }

                if (result.IsLockedOut)
                    return BadRequest(new { Message = "Account is locked out." });

                return BadRequest(new { Message = "Invalid login attempt." });
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return StatusCode(500, new { Message = "An error occurred during login", Error = ex.Message });
            }
        }
    }
}