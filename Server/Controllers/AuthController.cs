using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Server.Enums;

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
                // Create the user
                var user = new User
                {
                    UserName = model.Username ?? model.Email,
                    Name = model.Name,
                    Email = model.Email,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = false
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        Message = "User creation failed",
                        Errors = result.Errors.Select(e => e.Description)
                    });
                }

                // Validate and assign role
                if (!Enum.TryParse<UserRoles>(model.UserType, true, out UserRoles parsedRole))
                {
                    return BadRequest(new { Message = $"Invalid role specified. Valid roles are: {string.Join(", ", Enum.GetNames<UserRoles>())}" });
                }

                await _userManager.AddToRoleAsync(user, parsedRole.ToString());

                // Create related data
                if (parsedRole == UserRoles.vendor)
                {
                    var vendor = new Vendor
                    {
                        ShopId = model.ShopId!,
                        ShopName = model.ShopName!,
                        ShopAddress = model.ShopAddress!,
                        ShopLogo = model.ShopLogo,
                        Popularity = 0,
                        Id = user.Id
                    };
                    await _context.Vendors.AddAsync(vendor);
                }
                else if (parsedRole == UserRoles.customer)
                {
                    var customer = new Customer
                    {
                        Username = model.Username!,
                        ProfilePicture = model.ProfilePicture,
                        Id = user.Id
                    };
                    await _context.Customers.AddAsync(customer);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    Message = "Registration successful",
                    User = new { user.Id, user.Name, user.Email, Role = parsedRole.ToString() }
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

                    // getting user role : 
                    var roles = await _userManager.GetRolesAsync(user);


                    // Create claims for the 
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Email, user.Email!),
                        new Claim(ClaimTypes.Name, user.UserName!)
                    };

                    // Add role claims
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }


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
    }
}