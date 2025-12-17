using System.ComponentModel.DataAnnotations;
using BadBoys.DAL.Enums;

namespace BadBoys.DAL.Entities
{
    public class Media
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public int? Year { get; set; }

        [Required]
        public MediaType Type { get; set; } = MediaType.Movie;

        [MaxLength(100)]
        public string Genre { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Author { get; set; } = string.Empty; // Director/Developer/Artist/Writer
        
        [MaxLength(100)]
        public string Publisher { get; set; } = string.Empty; // Studio/Company/Label

        public string Description { get; set; } = string.Empty;

        [Range(0, 10)]
        public double? Rating { get; set; }

        public MediaStatus Status { get; set; } = MediaStatus.Planned;

        public string PersonalNotes { get; set; } = string.Empty; // Renamed from Notes

        public string CoverImagePath { get; set; } = string.Empty;

        public int? DurationMinutes { get; set; } // For movies/games
        public int? PageCount { get; set; } // For books
        public int? TrackCount { get; set; } // For music albums

        public DateTime? DateAdded { get; set; } = DateTime.Now;
        public DateTime? DateCompleted { get; set; }

        // Collections
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

        // Foreign keys
        public int UserId { get; set; } = 1;

        // Navigation properties
        public virtual User? User { get; set; }
    }
}
