using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BadBoys.DAL.Enums;

namespace BadBoys.DAL.Entities
{
    [Table("Media")] 
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
        public string Author { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Publisher { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0, 10)]
        public double? Rating { get; set; }

        public MediaStatus Status { get; set; } = MediaStatus.Planned;

        public string PersonalNotes { get; set; } = string.Empty;

        public string CoverImagePath { get; set; } = string.Empty;

        public int? DurationMinutes { get; set; }
        public int? PageCount { get; set; }
        public int? TrackCount { get; set; }

        public DateTime? DateAdded { get; set; } = DateTime.Now;
        public DateTime? DateCompleted { get; set; }

        // Collections
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

        // Foreign keys
        public int UserId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]  // ADD THIS LINE
        public virtual User? User { get; set; }
    }
}
