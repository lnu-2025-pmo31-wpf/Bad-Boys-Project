namespace BadBoys.DAL.Entities;

public class Media
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Genre { get; set; } = "";
    public int Year { get; set; }
    public string Type { get; set; } = ""; // Movies / Games / Music / Books
    public int DurationMinutes { get; set; }
    public double Rating { get; set; }
    public string ImageUrl { get; set; } = "";
}
