using System;
using System.Collections.Generic;
namespace App.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string? Email { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<MediaItem> MediaItems { get; set; } = new List<MediaItem>();
        public ICollection<Log> Logs { get; set; } = new List<Log>();
    }
}
