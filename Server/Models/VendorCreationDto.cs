using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class VendorCreationDto
    {
        [Required(ErrorMessage = "ShopName is required.")]
        [StringLength(100, ErrorMessage = "ShopName must be between 2 and 100 characters.", MinimumLength = 2)]
        public string ShopName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ShopAddress is required.")]
        [StringLength(200, ErrorMessage = "ShopAddress must be between 5 and 200 characters.", MinimumLength = 5)]
        public string ShopAddress { get; set; } = string.Empty;

        public string? ShopId { get; set; }
        public string? ShopLogo { get; set; }
    }
}