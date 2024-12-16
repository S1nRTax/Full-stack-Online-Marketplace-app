using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Customer
    {
        public int CustomerId { get; set; } // Unique identifier for 

        public int PurchaseCount { get; set; }

        public double TotalSpent { get; set; }

        [ForeignKey("User")] 
        public string Id { get; set; } // Foreign key to link with User

        public User User { get; set; }// Navigation property
    }
}
