using System.ComponentModel.DataAnnotations;
namespace Server.Models
{
    public class RegisterDto
    {
        // Common fields for all users
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name must be between 2 and 50 characters.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "User type is required.")]
        [RegularExpression("Vendor|Customer", ErrorMessage = "UserType must be either 'Vendor' or 'Customer'.")]
        public string UserType { get; set; } // "Vendor" or "Customer"

        // Vendor-specific fields
        [RequiredIf("UserType", "Vendor", ErrorMessage = "ShopId is required for vendors.")]
        public string? ShopId { get; set; }

        [RequiredIf("UserType", "Vendor", ErrorMessage = "ShopName is required for vendors.")]
        [StringLength(100, ErrorMessage = "ShopName must be between 2 and 100 characters.", MinimumLength = 2)]
        public string? ShopName { get; set; }

        [RequiredIf("UserType", "Vendor", ErrorMessage = "ShopAddress is required for vendors.")]
        [StringLength(200, ErrorMessage = "ShopAddress must be between 5 and 200 characters.", MinimumLength = 5)]
        public string? ShopAddress { get; set; }

        public string? ShopLogo { get; set; } // Optional for vendors

        // Customer-specific fields
        [RequiredIf("UserType", "Customer", ErrorMessage = "Username is required for customers.")]
        [StringLength(30, ErrorMessage = "Username must be between 3 and 30 characters.", MinimumLength = 3)]
        public string? Username { get; set; }

        public string? ProfilePicture { get; set; } // Optional for customers
    }
}