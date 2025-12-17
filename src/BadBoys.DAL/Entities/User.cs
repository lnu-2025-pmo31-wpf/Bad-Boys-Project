// File: src/BadBoys.DAL/Entities/User.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BadBoys.DAL.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Role { get; set; } = "User";
        
        public virtual ICollection<Media> MediaItems { get; set; } = new List<Media>();
    }
}
