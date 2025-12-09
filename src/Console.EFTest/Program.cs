using App.Data;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("EF Test starting...");

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite($"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "media.db")}")
    .Options;

using var db = new AppDbContext(options);

// виконаємо EnsureCreated як швидкий тест (якщо хочеш міграції — цей крок необов'язковий)
db.Database.EnsureCreated();

Console.WriteLine("Database path: " + Path.Combine(Directory.GetCurrentDirectory(), "media.db"));
Console.WriteLine("Tables:");
foreach (var t in db.Model.GetEntityTypes())
    Console.WriteLine(" - " + t.GetTableName());

Console.WriteLine("Done.");

