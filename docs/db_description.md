# Опис структури бази даних (Media Collection Manager)

**Провайдер:** SQLite — портативність, офлайн, легка інтеграція з EF Core.

## Users
- Id — INTEGER PRIMARY KEY AUTOINCREMENT
- Username — TEXT NOT NULL UNIQUE
- PasswordHash — TEXT NOT NULL
- Email — TEXT
- Role — TEXT DEFAULT 'User'
- CreatedAt — DATETIME DEFAULT CURRENT_TIMESTAMP

## MediaItems
- Id — INTEGER PRIMARY KEY AUTOINCREMENT
- UserId — INTEGER NOT NULL (FK → Users.Id)
- Title — TEXT NOT NULL
- Year — INTEGER
- Type — TEXT NOT NULL (Movie, Game, Music, Book)
- Genre — TEXT
- Author — TEXT
- Description — TEXT
- Rating — REAL
- Status — TEXT (Watched, InProgress, Planned, Completed)
- CoverImagePath — TEXT
- CreatedAt — DATETIME DEFAULT CURRENT_TIMESTAMP

## Tags
- Id — INTEGER PRIMARY KEY AUTOINCREMENT
- MediaItemId — INTEGER NOT NULL (FK → MediaItems.Id)
- Name — TEXT NOT NULL

## Logs
- Id — INTEGER PRIMARY KEY AUTOINCREMENT
- UserId — INTEGER NOT NULL (FK → Users.Id)
- MediaItemId — INTEGER NULL (FK → MediaItems.Id)
- Action — TEXT NOT NULL
- Date — DATETIME DEFAULT CURRENT_TIMESTAMP
