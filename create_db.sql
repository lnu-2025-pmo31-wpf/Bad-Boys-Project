-- Create Users table
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Role TEXT DEFAULT 'User',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Create Media table
CREATE TABLE Media (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    Year INTEGER NOT NULL,
    Type TEXT NOT NULL,
    Genre TEXT,
    Author TEXT,
    Description TEXT,
    Rating REAL DEFAULT 0,
    Status TEXT DEFAULT 'Planned',
    Notes TEXT,
    CoverImagePath TEXT,
    DurationMinutes INTEGER DEFAULT 0,
    UserId INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Insert default user (password: test123)
INSERT INTO Users (Username, PasswordHash, Role) 
VALUES ('test', 'mZ9cXq3iLb2Pw8sR5vY7tG1h4jK6nM0pQ2wE4rT6yU8i', 'User');

-- Insert sample media
INSERT INTO Media (Title, Year, Type, Genre, Author, Description, Rating, Status, UserId)
VALUES 
    ('The Matrix', 1999, 'Movie', 'Sci-Fi', 'The Wachowskis', 'A computer hacker learns about the true nature of reality', 8.7, 'Watched', 1),
    ('The Legend of Zelda', 2017, 'Game', 'Adventure', 'Nintendo', 'Open-world action-adventure game', 9.5, 'Completed', 1);
