using System.ComponentModel.DataAnnotations;

namespace BadBoys.DAL.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        public int MediaId { get; set; }
        public virtual Media? Media { get; set; }
    }
}
