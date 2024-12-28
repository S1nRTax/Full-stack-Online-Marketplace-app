using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Data;
using Server.Models;
using Server.Services;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransitionController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IUserTransitionService _userTransitionService;
        private readonly ILogger<TransitionController> _logger;

        public TransitionController(UserManager<User> userManager,
                                    SignInManager<User> signInManager,
                                    IConfiguration configuration,
                                    ApplicationDbContext context,
                                    IUserTransitionService userTransitionService,
                                    ILogger<TransitionController> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userTransitionService = userTransitionService ?? throw new ArgumentNullException(nameof(userTransitionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpPost("become-vendor/{userId}")]
        [Authorize]
        public async Task<IActionResult> BecomeVendor([FromRoute] string userId, [FromBody] VendorCreationDto vendorDetails)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("User ID is required.");

            if (vendorDetails == null)
                return BadRequest("Vendor details cannot be null.");

            try
            {
                // Find the user by ID
                var userInfo = await _userManager.FindByIdAsync(userId);
                if (userInfo == null)
                    return NotFound($"User with ID {userId} not found.");

                // Validate vendor creation details
                var validationResult = await ValidateVendorDetails(vendorDetails);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.ErrorMessage);

                // Attempt to transition user to vendor
                var result = await _userTransitionService.BecomeVendorAsync(userInfo.Id.ToString(), vendorDetails);

                // Handle vendor creation failure
                if (!result)
                {
                    if (userInfo.Vendor != null)
                        return Conflict("User is already registered as a vendor.");

                    return StatusCode(500, "Failed to create vendor profile.");
                }

                // Ensure vendor properties are initialized
                userInfo.Vendor ??= new Vendor(); // Null-coalescing assignment if Vendor is null
                userInfo.Vendor.Popularity = 0;

                // Update user with new vendor information
                await _userManager.UpdateAsync(userInfo);

                // Return successful vendor creation response
                return CreatedAtAction(nameof(BecomeVendor), new
                {
                    Message = "User successfully transitioned to vendor.",
                    userInfo.HasShop,
                    VendorId = userInfo.Vendor.ShopId,
                    ShopName = userInfo.Vendor.ShopName,
                    ShopAddress = userInfo.Vendor.ShopAddress,
                    ShopDescription = userInfo.Vendor.ShopDescription,
                    Popularity = userInfo.Vendor.Popularity
                });
            }
            catch (Exception ex)
            {
                // Comprehensive error logging
                _logger.LogError(ex, $"Unexpected error converting user {userId} to vendor: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred. Please contact support.");
            }
        }

        // Vendor details validation method
        public async Task<(bool IsValid, string ErrorMessage)> ValidateVendorDetails(VendorCreationDto vendorDetails)
        {
            if (string.IsNullOrWhiteSpace(vendorDetails.ShopName))
                return (false, "Shop name is required.");

            if (vendorDetails.ShopName.Length < 3 || vendorDetails.ShopName.Length > 100)
                return (false, "Shop name must be between 3 and 100 characters.");

            if (string.IsNullOrWhiteSpace(vendorDetails.ShopAddress))
                return (false, "Shop address is required.");

            if (vendorDetails.ShopAddress.Length > 250)
                return (false, "Shop address is too long.");

            // Additional validations can be added here

            // Check if the current user has a shop
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentUser == null)
            {
                return (false, "Current user not found.");
            }

            currentUser.HasShop = true;
            await _userManager.UpdateAsync(currentUser);

            return (true, null);
        }



        // API endpoint to validate a vendor.
        [HttpGet("validateVendor")]
        [Authorize]
        public async Task<IActionResult> ValidateVendor()
        {
            try
            {
                // get the current logged in User with claims to load the navigations properties.
                var user = await _userManager.Users
                 .Include(u => u.Vendor)
                 .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier) );

                if (user == null)
                    return Unauthorized();

    
                if (!user.HasShop)
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    isVendor = user.HasShop,
                    VendorDetails = user.HasShop ? new
                    {
                        user.Vendor.ShopId,
                        user.Vendor.ShopName,
                        user.Vendor.ShopAddress,
                        user.Vendor.ShopDescription
                    } : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating vendor status");
                return StatusCode(500, "An error occurred while validating vendor status");
            }
        }


    }
}
