

using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Vendor
    {
        public int VendorId { get; set; } // Unique identifier for the vendor
        public string ShopId { get; set; } // Unique shop identifier
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string? ShopLogo { get; set; } // URL or path to the logo
        public int Popularity { get; set; } // Metric for popularity (e.g., number of sales)

        // Foreign key to link with User
        [ForeignKey("User")]
        public string? Id { get; set; }
        public User User { get; set; } // Navigation property
    }

}
