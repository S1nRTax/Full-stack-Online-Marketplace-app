using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class CreatePostDto
    {
        [Required(ErrorMessage ="Title is required")]
        [StringLength(30, MinimumLength = 5 , ErrorMessage = "Title must be between 5 and 30 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage ="Description is required")]
        [StringLength(300, MinimumLength = 3, ErrorMessage ="Description must be between 3 and 300 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage ="Price is required")]
        public double Price { get; set; }

        [Required(ErrorMessage ="Picture is required")]
        public string picture { get; set; }

    }
}
