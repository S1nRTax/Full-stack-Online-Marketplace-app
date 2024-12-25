using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class PostModel
    {
        [Key]
        public string PostId { get; set; }
        public string PostTitle { get; set; } = string.Empty;

        public string PostImagePath { get; set; } = string.Empty;

        public int PostUpVotes { get; set; }
        public double PostPriceTag { get; set; }

        public DateTime PostCreatedAt { get; set; }

        public string VendorId { get; set; }

        // Navigation Properties.
        public Vendor Vendor { get; set; }
    }
}
