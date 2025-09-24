# Bad-Boys-Project
Tonizzo Simoe, Pryadko Andrii &amp; Syrota Denys Project

“Personal Media Collection Manager” - a desktop application for organizing personal collections of media content (movies, games, music, books).

• Application Goal

Create a convenient tool for people who want to organize their personal collection of media - movies, TV shows, games, music, or books.
The app should provide:

   ◦ a simple catalog with quick search and filtering;

   ◦ storage of complete information (title, year, genre, rating, description, cover image);

   ◦ status markers (“watched / playing / want to watch / completed”);

   ◦ personal notes and reviews;

   ◦ collection statistics (e.g., how many sci-fi movies, which games are still uncompleted);

   ◦ export/import of the database (CSV, Excel, PDF).

• Problems It Solves

   ◦ Chaotic storage of information: many users record movies or games in notes, Excel sheets, or even notebooks. This is inconvenient and doesn’t provide statistics.

   ◦ Lack of local solutions: most modern services (like Letterboxd, MyAnimeList, Goodreads) are online and require an account. Many users value full privacy.

   ◦ Difficulty in searching and systematizing: with a large collection, it’s hard to find a specific media item without filters and search.

   ◦ Lack of universality: existing apps are usually specialized (only for movies, or only for books), while here everything can be stored in one place.

• Types of Users

   ◦ Cinephiles - keep lists of watched/planned movies and TV shows.

   ◦ Gamers - catalog games, mark which are completed and which are in plans.

   ◦ Music lovers - organize music albums and tracks.

   ◦ Book lovers - build their personal e-library (with comments and ratings).

   ◦ Collectors - people who simply enjoy maintaining a full inventory of their collection (for organization or archiving).

• Main Application Requirements

  Functional

   ◦ Authentication (each user can have their own database, allowing multiple people to use the app).

   ◦ Media catalog with categories (Movies, Games, Music, Books).

   ◦ Media item card with detailed information:

   ◦ title, year, genre, author/director/performer, description, rating, personal notes;

   ◦ cover image (add from local file or auto-download from the internet via API).

   ◦ Search and filters (by genre, year, status, rating).

   ◦ Status markers: “Watched/Completed,” “In progress,” “Planned.”

   ◦ Statistics: number of media items in each category, top genres, rating charts.

   ◦ Export to CSV/Excel for backup or analysis.

   ◦ PDF report generation (e.g., “My Movie Collection 2025”).

   ◦ Data import from files (for quick bulk collection entry).

  Non-functional

   ◦ UI: WPF with MVVM, user-friendly and modern design (lists + cards + statistics).

   ◦ Database: SQLite (portability) or SQL Server Express (for multi-user mode).

   ◦ Entity Framework Core for CRUD operations.

   ◦ ADO.NET for bulk operations (import/export) and optimized queries (e.g., statistics).

   ◦ Security: user password hashing.

   ◦ Performance: handle several thousand records without noticeable lag.

• Analysis of Similar Applications

Letterboxd (online platform for movies)

Pros: beautiful interface, social features (following, comments, lists).

Cons: only for movies; requires internet; some features are paid; no local database.

Core features: movie lists, ratings, reviews, social interaction.

Goodreads (online book service, owned by Amazon)

Pros: huge book database, recommendations, reviews.

Cons: only for books; no offline access; lots of ads.

Core features: tracking reading progress, ratings, lists, social features.

GCstar (desktop open-source cataloging tool)

Pros: supports various collection types (movies, games, music, comics, etc.).

Cons: outdated interface, no active support, limited reports.

Core features: cataloging with tags and search, simple reports.

Conclusion of analysis: Most solutions are either online and narrowly specialized (Letterboxd, Goodreads), or universal but outdated (GCstar). Our application will be local, universal, modern, and private.

• Technical Structure

   ◦ Database Tables

   ◦ Users (Id, Username, PasswordHash, Role)

   ◦ MediaItems (Id, Title, Year, Type, Genre, Author, Description, Rating, Status, CoverImagePath, UserId)

   ◦ Tags (Id, Name, MediaItemId)

   ◦ Logs (Id, Action, Date, UserId, MediaItemId)

Main WPF Screens

   ◦ Login / Register - access personal collection.

   ◦ Main dashboard - media lists with filters (grid + search panel).

   ◦ Media card - detailed info, editing, cover image.

   ◦ Add/Import - quick data entry (manual or from CSV).

   ◦ Statistics - charts/graphs (e.g., number of media items by genre).

   ◦ Export/Import - PDF/Excel/CSV.

   ◦ User settings - profile, backups.

• Possible Extensions

   ◦ API integration (OMDb API - auto-fetch movie data; IGDB API - for games).

   ◦ Adding QR codes for items (e.g., to share lists).

   ◦ Easy synchronization across PCs via local network.
