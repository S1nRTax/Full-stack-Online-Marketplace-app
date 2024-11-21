namespace Server.Models
{
    public class Customer
    {
        public int CustomerId { get; set; } // Unique identifier for the customer
        public string Username { get; set; }

        public string ProfilePicture { get; set; } // URL or path to the profile picture

        // Foreign key to link with User
        public string UserId { get; set; }
        public User User { get; set; } // Navigation property
    }
}
