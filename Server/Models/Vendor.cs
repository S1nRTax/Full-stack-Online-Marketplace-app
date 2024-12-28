

using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Vendor
    {
        public string ShopId { get; set; } // Unique shop identifier
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string ShopDescription { get; set; }
        public string? ShopLogo { get; set; } // URL or path to the logo
        public int Popularity { get; set; } // Metric for popularity (e.g., number of sales)

        public ICollection<PostModel> Posts { get; set; }

        // Foreign key to link with User
        [ForeignKey("User")]
        public string? Id { get; set; }

        // Navigation Properties
        public User User { get; set; } 

    }

}
