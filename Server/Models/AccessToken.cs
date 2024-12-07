using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class AccessToken
    {
        [Key]
        public string Token {  get; set; }   
        public DateTime CreateAt { get; set; }  
        public DateTime ExpiresAt { get; set; }

        // Foreign Key to User
        public string UserId { get; set; }

        // Navigation property to User
        public virtual User User { get; set; }
    }
}
