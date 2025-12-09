using System;
using System.Collections.Generic;
namespace App.Data.Models
{
    public class Log
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? MediaItemId { get; set; }
        public string Action { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public MediaItem? MediaItem { get; set; }
    }
}
