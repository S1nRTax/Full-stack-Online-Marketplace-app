namespace Server.Models
{
    public class Post
    {
        string PostId { get; set; }
        public string PostAuthor { get; set; } = string.Empty;
        public  DateTime PostCreatedAt { get; set; }

        public string PostTitle { get; set; } = string.Empty;
        public string PostDescription { get; set; } = string.Empty;

        public double Price { get; set; }
        

    }
}
