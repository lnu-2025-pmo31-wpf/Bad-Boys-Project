using Microsoft.Data.Sqlite;
using System;
using System.IO;

class Program
{
    static string DbPath => Path.Combine(Directory.GetCurrentDirectory(), "media.db");
    static string ConnStr => $"Data Source={DbPath}";
    static Random rnd = new Random();

    static void Main()
    {
        Console.WriteLine($"DB path: {DbPath}");
        EnsureDatabase();
        SeedUsers(5);
        SeedMediaItems(40);
        Console.WriteLine("Seeding complete.");
    }

    static void EnsureDatabase()
    {
        if (!File.Exists(DbPath))
        {
            var sqlFile = Path.Combine("..", "..", "database", "create_db.sql");
            if (!File.Exists(sqlFile)) sqlFile = Path.Combine("database", "create_db.sql");
            var sql = File.ReadAllText(sqlFile);
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            conn.Close();
            Console.WriteLine("Created DB via SQL script.");
        }
        else
        {
            Console.WriteLine("DB already exists.");
        }
    }

    static void SeedUsers(int count)
    {
        using var conn = new SqliteConnection(ConnStr);
        conn.Open();
        for (int i = 1; i <= count; i++)
        {
            var username = $"user{i}";
            var pass = $"pass{i}";
            var hash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(pass));
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT OR IGNORE INTO Users (Username, PasswordHash, Email, Role) VALUES (@u,@p,@e,@r);";
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", hash);
            cmd.Parameters.AddWithValue("@e", $"{username}@example.com");
            cmd.Parameters.AddWithValue("@r", "User");
            cmd.ExecuteNonQuery();
        }
        conn.Close();
        Console.WriteLine("Users seeded.");
    }

    static void SeedMediaItems(int count)
    {
        var types = new[] { "Movie", "Game", "Music", "Book" };
        var genres = new[] { "Sci-Fi", "Action", "Drama", "Comedy", "RPG", "Adventure", "Pop", "Rock", "Fantasy" };
        using var conn = new SqliteConnection(ConnStr);
        conn.Open();

        using var getCmd = conn.CreateCommand();
        getCmd.CommandText = "SELECT Id FROM Users LIMIT 1";
        var firstUserId = Convert.ToInt32(getCmd.ExecuteScalar() ?? 1);

        for (int i = 1; i <= count; i++)
        {
            var title = $"Sample Title {i}";
            var year = rnd.Next(1980, 2026);
            var type = types[rnd.Next(types.Length)];
            var genre = genres[rnd.Next(genres.Length)];
            var author = type == "Book" ? $"Author {i}" : (type == "Movie" ? $"Director {i}" : $"Artist {i}");
            var desc = $"Auto-generated description for {title}";
            var rating = Math.Round(rnd.NextDouble() * 10, 1);
            var status = (i % 3 == 0) ? "Planned" : (i % 2 == 0 ? "InProgress" : "Completed");

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO MediaItems (UserId, Title, Year, Type, Genre, Author, Description, Rating, Status) 
                                VALUES (@uid,@title,@year,@type,@genre,@author,@desc,@rating,@status);";
            cmd.Parameters.AddWithValue("@uid", firstUserId);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@genre", genre);
            cmd.Parameters.AddWithValue("@author", author);
            cmd.Parameters.AddWithValue("@desc", desc);
            cmd.Parameters.AddWithValue("@rating", rating);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.ExecuteNonQuery();

            using var lastCmd = conn.CreateCommand();
            lastCmd.CommandText = "SELECT last_insert_rowid();";
            var mediaId = Convert.ToInt32(lastCmd.ExecuteScalar());

            int tagCount = rnd.Next(1, 4);
            for (int t = 0; t < tagCount; t++)
            {
                using var tagCmd = conn.CreateCommand();
                tagCmd.CommandText = "INSERT INTO Tags (MediaItemId, Name) VALUES (@mid,@name)";
                tagCmd.Parameters.AddWithValue("@mid", mediaId);
                tagCmd.Parameters.AddWithValue("@name", $"{genre}-tag{t+1}");
                tagCmd.ExecuteNonQuery();
            }

            using var logCmd = conn.CreateCommand();
            logCmd.CommandText = "INSERT INTO Logs (UserId, MediaItemId, Action) VALUES (@uid,@mid,@act)";
            logCmd.Parameters.AddWithValue("@uid", firstUserId);
            logCmd.Parameters.AddWithValue("@mid", mediaId);
            logCmd.Parameters.AddWithValue("@act", "SeedInsert");
            logCmd.ExecuteNonQuery();
        }
        conn.Close();
        Console.WriteLine("Media items seeded.");
    }
}

