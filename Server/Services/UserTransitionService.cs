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

                string profilePicture = string.IsNullOrWhiteSpace(customerDetails.ProfilePicture)
                    ? "/images/user.png"
                    : customerDetails.ProfilePicture;

                var newCustomer = new Customer
                {
                    Id = userId,
                    ProfilePicture = profilePicture,
                    Username = user.UserName 
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
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                if (await IsVendorAsync(userId)) return false;

                
                string shopLogo = string.IsNullOrWhiteSpace(vendorDetails.ShopLogo)
                    ? "/images/shop.png"
                    : vendorDetails.ShopLogo;

                var newVendor = new Vendor
                {
                    Id = userId,
                    ShopName = vendorDetails.ShopName,
                    ShopAddress = vendorDetails.ShopAddress,
                    ShopId = Guid.NewGuid().ToString(),
                    ShopLogo = shopLogo,
                    Popularity = 0
                };

                _context.Vendors.Add(newVendor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error converting user {userId} to a vendor");
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