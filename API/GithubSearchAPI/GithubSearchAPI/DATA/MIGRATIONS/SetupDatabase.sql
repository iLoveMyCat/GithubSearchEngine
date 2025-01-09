-- Check if the database exists
IF DB_ID('GITHUB_REPOSITORY') IS NULL
BEGIN
    CREATE DATABASE GITHUB_REPOSITORY;
    PRINT 'Database created successfully.';
END
ELSE
BEGIN
    PRINT 'Database already exists.';
END;

-- Switch to the database
USE GITHUB_REPOSITORY;

-- Create Users table if it does not exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(100) NOT NULL
    );
    PRINT 'Users table created successfully.';
END
ELSE
BEGIN
    PRINT 'Users table already exists.';
END;

-- Create Favorites table if it does not exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Favorites')
BEGIN
    CREATE TABLE Favorites (
        FavoriteId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        RepositoryName NVARCHAR(255) NOT NULL,
        RepositoryUrl NVARCHAR(500) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
    PRINT 'Favorites table created successfully.';
END
ELSE
BEGIN
    PRINT 'Favorites table already exists.';
END;

-- Create stored procedures if they do not exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'stp_validateUser')
BEGIN
    EXEC('
        CREATE PROCEDURE stp_validateUser
            @Username NVARCHAR(50),
            @PasswordHash NVARCHAR(100)
        AS
        BEGIN
            SELECT Id, Username
            FROM Users
            WHERE Username = @Username AND PasswordHash = @PasswordHash;
        END;
    ');
    PRINT 'Stored procedure stp_validateUser created successfully.';
END
ELSE
BEGIN
    PRINT 'Stored procedure stp_validateUser already exists.';
END;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'stp_AddFavorite')
BEGIN
    EXEC('
        CREATE PROCEDURE stp_AddFavorite
            @FavoriteId INT OUTPUT,
            @UserId INT,
            @RepositoryName NVARCHAR(255),
            @RepositoryUrl NVARCHAR(500)
        AS
        BEGIN
            INSERT INTO Favorites (UserId, RepositoryName, RepositoryUrl, CreatedAt)
            VALUES (@UserId, @RepositoryName, @RepositoryUrl, GETDATE());

            SELECT @FavoriteId = SCOPE_IDENTITY();
        END;
    ');
    PRINT 'Stored procedure stp_AddFavorite created successfully.';
END
ELSE
BEGIN
    PRINT 'Stored procedure stp_AddFavorite already exists.';
END;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'stp_GetFavorites')
BEGIN
    EXEC('
        CREATE PROCEDURE stp_GetFavorites
            @UserId INT
        AS
        BEGIN
            SELECT RepositoryName, RepositoryUrl
            FROM Favorites
            WHERE UserId = @UserId;
        END;
    ');
    PRINT 'Stored procedure stp_GetFavorites created successfully.';
END
ELSE
BEGIN
    PRINT 'Stored procedure stp_GetFavorites already exists.';
END;

-- Create login if it does not exist
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'GitHubSearchAppLogin')
BEGIN
    EXEC('CREATE LOGIN GitHubSearchAppLogin WITH PASSWORD = ''{PASSWORD}''');
    PRINT 'Login GitHubSearchAppLogin created successfully.';
END
ELSE
BEGIN
    PRINT 'Login GitHubSearchAppLogin already exists.';
END;

-- Create user for the login if it does not exist
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'GitHubSearchAppUser')
BEGIN
    CREATE USER GitHubSearchAppUser FOR LOGIN GitHubSearchAppLogin;
    PRINT 'User GitHubSearchAppUser created successfully.';
END
ELSE
BEGIN
    PRINT 'User GitHubSearchAppUser already exists.';
END;

-- Create role and permissions if it does not exist
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ProcedureExecutor')
BEGIN
    CREATE ROLE ProcedureExecutor;
    GRANT EXECUTE TO ProcedureExecutor;
    DENY SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO ProcedureExecutor;
    ALTER ROLE ProcedureExecutor ADD MEMBER GitHubSearchAppUser;
    PRINT 'Role ProcedureExecutor created successfully.';
END
ELSE
BEGIN
    PRINT 'Role ProcedureExecutor already exists.';
END;
