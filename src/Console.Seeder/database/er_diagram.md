erDiagram
    USERS ||--o{ MEDIA_ITEMS : "owns"
    USERS ||--o{ LOGS : "creates"
    MEDIA_ITEMS ||--o{ TAGS : "has"
    MEDIA_ITEMS ||--o{ LOGS : "is target of"

    USERS {
        int Id PK
        string Username
        string PasswordHash
        string Email
        string Role
        datetime CreatedAt
    }

    MEDIA_ITEMS {
        int Id PK
        int UserId FK
        string Title
        int Year
        string Type
        string Genre
        string Author
        string Description
        real Rating
        string Status
        string CoverImagePath
        datetime CreatedAt
    }

    TAGS {
        int Id PK
        int MediaItemId FK
        string Name
    }

    LOGS {
        int Id PK
        int UserId FK
        int MediaItemId FK
        string Action
        datetime Date
    }

