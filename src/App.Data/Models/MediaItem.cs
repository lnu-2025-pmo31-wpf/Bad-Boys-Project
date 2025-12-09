namespace App.Data.Models
{
    public class MediaItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = "";
        public int? Year { get; set; }
        public string Type { get; set; } = ""; // Movie, Game, Music, Book
        public string? Genre { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public double? Rating { get; set; }
        public string? Status { get; set; }
        public string? CoverImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<Log> Logs { get; set; } = new List<Log>();
    }
}
