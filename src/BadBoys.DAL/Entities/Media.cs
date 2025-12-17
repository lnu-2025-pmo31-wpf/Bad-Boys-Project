using System.ComponentModel.DataAnnotations;

namespace BadBoys.DAL.Entities
{
    public class Media
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public int Year { get; set; }
        
        [Required]
        public string Type { get; set; } = string.Empty;
        
        public string Genre { get; set; } = string.Empty;
        
        public string Author { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Range(0, 10)]
        public double Rating { get; set; }
        
        public string Status { get; set; } = "Planned";
        
        public string Notes { get; set; } = string.Empty;
        
        public string CoverImagePath { get; set; } = string.Empty;
        
        public int DurationMinutes { get; set; }
        
        // Foreign key
        public int UserId { get; set; } = 1;
        
        // Navigation property
        public virtual User? User { get; set; }
    }
}
