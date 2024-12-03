using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Customer
    {
        public int CustomerId { get; set; } // Unique identifier for the 
        public string Username { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; } // URL or path to the profile picture


        [ForeignKey("User")] 
        public string Id { get; set; } // Foreign key to link with User

        public User User { get; set; }// Navigation property
    }
}
