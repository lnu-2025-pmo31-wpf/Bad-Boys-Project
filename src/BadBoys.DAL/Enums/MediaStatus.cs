namespace BadBoys.DAL.Enums
{
    public enum MediaStatus
    {
        Planned = 0,      // Want to watch/play/read
        InProgress = 1,   // Currently watching/playing/reading
        Completed = 2,    // Finished watching/playing/reading
        Dropped = 3,      // Stopped before finishing
        OnHold = 4        // Paused temporarily
    }

    public enum MediaType
    {
        Movie = 0,
        TVShow = 1,
        Game = 2,
        Music = 3,
        Book = 4,
        Other = 5
    }
}
