using Server.Models;

namespace Server.Services
{
    public interface IUserTransitionService
    {
        Task<bool> BecomeCustomerAsync(string userId, CustomerCreationDto customerDetails);
        Task<bool> BecomeVendorAsync(string userId, VendorCreationDto vendorDetails);
        Task<bool> IsCustomerAsync(string userId);
        Task<bool> IsVendorAsync(string userId);
    }
}
