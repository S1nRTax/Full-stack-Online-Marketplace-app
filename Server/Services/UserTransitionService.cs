using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
namespace Server.Services
{
    public class UserTransitionService : IUserTransitionService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserTransitionService> _logger;
        public UserTransitionService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ILogger<UserTransitionService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<bool> BecomeCustomerAsync(string userId, CustomerCreationDto customerDetails)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;
                if (await IsCustomerAsync(userId)) return false;
                
                var newCustomer = new Customer
                {
                    Id = userId,
                    PurchaseCount = 0,
                    TotalSpent = 0,
                };
                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error converting user {userId} to customer");
                return false;
            }
        }
        public async Task<bool> BecomeVendorAsync(string userId, VendorCreationDto vendorDetails)
        {
            try
            {
                // Detailed logging
                _logger.LogInformation($"Attempting to convert user {userId} to vendor");

                // Find the user
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User {userId} not found");
                    return false;
                }

                // Check if already a vendor
                bool isAlreadyVendor = await IsVendorAsync(userId);
                if (isAlreadyVendor)
                {
                    _logger.LogWarning($"User {userId} is already a vendor");
                    return false;
                }

                // Prepare shop logo
                string shopLogo = string.IsNullOrWhiteSpace(vendorDetails.ShopLogo)
                    ? "/images/shop.png"
                    : vendorDetails.ShopLogo;

                // Create new vendor
                var newVendor = new Vendor
                {
                    Id = userId,
                    ShopId = Guid.NewGuid().ToString(),
                    ShopName = vendorDetails.ShopName,
                    ShopAddress = vendorDetails.ShopAddress,
                    ShopDescription = vendorDetails.ShopDescription,
                    ShopLogo = shopLogo,
                    Popularity = 0
                };

                // Add and save
                _context.Vendors.Add(newVendor);

                // Additional logging before save
                _logger.LogInformation($"Saving new vendor for user {userId}");

                int saveResult = await _context.SaveChangesAsync();

                // Log save result
                _logger.LogInformation($"SaveChanges result: {saveResult} entities saved");

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                // More specific exception handling
                _logger.LogError(dbEx, $"Database error converting user {userId} to vendor: {dbEx.Message}");

                // Log inner exception details
                if (dbEx.InnerException != null)
                {
                    _logger.LogError(dbEx.InnerException, "Inner exception details");
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error converting user {userId} to a vendor");
                return false;
            }
        }
        public async Task<bool> IsCustomerAsync(string userId)
        {
            return await _context.Customers.AnyAsync(c => c.Id == userId);
        }
        public async Task<bool> IsVendorAsync(string userId)
        {
            return await _context.Vendors.AnyAsync(v => v.Id == userId);
        }
    }
}