using Microsoft.Data.Sqlite;
using System;

class Program
{
    static string DbPath = @"C:\Users\38097\Bad-Boys-Project\src\Console.Seeder\media.db";

    static void Main()
    {
        Console.WriteLine("ADO.NET Console App running...\n");

        using var connection = new SqliteConnection($"Data Source={DbPath}");
        connection.Open();

        Console.WriteLine(" USERS ");
        DisplayTable(connection, "Users");

        Console.WriteLine(" MEDIA ITEMS ");
        DisplayTable(connection, "MediaItems");

        Console.WriteLine(" TAGS ");
        DisplayTable(connection, "Tags");

        Console.WriteLine(" LOGS ");
        DisplayTable(connection, "Logs");

        Console.WriteLine("\nGenerate test data? (y/n)");
        if (Console.ReadKey().Key == ConsoleKey.Y)
        {
            GenerateTestData(connection);
            Console.WriteLine("\nDONE!");
        }
    }

    static void DisplayTable(SqliteConnection connection, string table)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {table}";

        using var reader = cmd.ExecuteReader();
        int columnCount = reader.FieldCount;

        while (reader.Read())
        {
            for (int i = 0; i < columnCount; i++)
                Console.Write(reader.GetValue(i) + " | ");
            Console.WriteLine();
        }

        Console.WriteLine("\n");
    }

    static void GenerateTestData(SqliteConnection connection)
    {
        Console.WriteLine("\nGenerating test data...");

        InsertUsers(connection, 30);
        InsertMediaItems(connection, 30);
        InsertTags(connection, 30);
        InsertLogs(connection, 30);
    }

    static void InsertUsers(SqliteConnection conn, int count)
    {
        for (int i = 1; i <= count; i++)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "INSERT INTO Users (Username, PasswordHash, Email, Role) VALUES (" +
                $" 'test{i}', 'hash{i}', 'test{i}@gmail.com', 'User' )";
            cmd.ExecuteNonQuery();
        }
    }

    static void InsertMediaItems(SqliteConnection conn, int count)
    {
        var rnd = new Random();

        for (int i = 1; i <= count; i++)
        {
            int userId = rnd.Next(1, 6);

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "INSERT INTO MediaItems (UserId, Title, Year, Type, Genre, Author, Description, Rating, Status)" +
                $" VALUES ({userId}, 'Movie {i}', 2000 + {i % 20}, 'Movie', 'Action', 'Director {i}', 'Description {i}', {rnd.NextDouble() * 10}, 'Completed')";
            cmd.ExecuteNonQuery();
        }
    }

    static void InsertTags(SqliteConnection conn, int count)
    {
        var rnd = new Random();

        for (int i = 1; i <= count; i++)
        {
            int mediaId = rnd.Next(1, 30);

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO Tags (MediaItemId, Name) VALUES ({mediaId}, 'tag{i}')";
            cmd.ExecuteNonQuery();
        }
    }

    static void InsertLogs(SqliteConnection conn, int count)
    {
        var rnd = new Random();

        for (int i = 1; i <= count; i++)
        {
            int userId = rnd.Next(1, 6);
            int mediaId = rnd.Next(1, 30);

            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO Logs (UserId, MediaItemId, Action) VALUES ({userId}, {mediaId}, 'Viewed')";
            cmd.ExecuteNonQuery();
        }
    }
}
