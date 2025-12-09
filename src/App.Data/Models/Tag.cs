namespace App.Data.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int MediaItemId { get; set; }
        public string Name { get; set; } = "";

        public MediaItem? MediaItem { get; set; }
    }
}
