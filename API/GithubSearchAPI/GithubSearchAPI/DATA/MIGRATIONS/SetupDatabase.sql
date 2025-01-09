-- Check if the database exists and create it if not
IF DB_ID('GITHUB_TEST') IS NULL
BEGIN
    CREATE DATABASE GITHUB_TEST;
    PRINT 'Database "GITHUB_TEST" created successfully.';
END
ELSE
BEGIN
    PRINT 'Database "GITHUB_TEST" already exists.';
END;

-- Switch to the GITHUB_TEST database
USE GITHUB_TEST;

-- Create Users table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(100) NOT NULL
    );
    PRINT 'Table "Users" created successfully.';
END
ELSE
BEGIN
    PRINT 'Table "Users" already exists.';
END;

-- Create Favorites table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Favorites' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Favorites (
        FavoriteId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        RepositoryName NVARCHAR(255) NOT NULL,
        RepositoryUrl NVARCHAR(500) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
    PRINT 'Table "Favorites" created successfully.';
END
ELSE
BEGIN
    PRINT 'Table "Favorites" already exists.';
END;

-- Create stored procedure: stp_getUserByUsername
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'stp_getUserByUsername')
BEGIN
    EXEC('
        CREATE PROCEDURE stp_getUserByUsername
            @Username NVARCHAR(50)
        AS
        BEGIN
            SET NOCOUNT ON;

            SELECT Id, Username, PasswordHash
            FROM Users
            WHERE Username = @Username;
        END;
    ');
    PRINT 'Stored procedure "stp_getUserByUsername" created successfully.';
END
ELSE
BEGIN
    PRINT 'Stored procedure "stp_getUserByUsername" already exists.';
END;

-- Create stored procedure: stp_createUser
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'stp_createUser')
BEGIN
    EXEC('
        CREATE PROCEDURE [dbo].[stp_createUser]
            @Username NVARCHAR(50),
            @PasswordHash NVARCHAR(100)
        AS
        BEGIN
            SET NOCOUNT ON;

            -- Check if the username already exists
            IF EXISTS (SELECT 1 FROM dbo.Users WHERE Username = @Username)
            BEGIN
                THROW 50001, ''Username already exists.'', 1;
            END;

            -- Insert the new user
            INSERT INTO dbo.Users (Username, PasswordHash)
            VALUES (@Username, @PasswordHash);

            -- Return the ID of the newly created user
            SELECT SCOPE_IDENTITY() AS UserId;
        END;
    ');
    PRINT 'Stored procedure "stp_createUser" created successfully.';
END
ELSE
BEGIN
    PRINT 'Stored procedure "stp_createUser" already exists.';
END;

-- Create stored procedure: stp_AddFavorite
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
            SET NOCOUNT ON;

            INSERT INTO Favorites (UserId, RepositoryName, RepositoryUrl, CreatedAt)
            VALUES (@UserId, @RepositoryName, @RepositoryUrl, GETDATE());

            SELECT @FavoriteId = SCOPE_IDENTITY();
        END;
    ');
    PRINT 'Stored procedure "stp_AddFavorite" created successfully.';
END
ELSE
BEGIN
    PRINT 'Stored procedure "stp_AddFavorite" already exists.';
END;

-- Create stored procedure: stp_GetFavorites
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'stp_GetFavorites')
BEGIN
    EXEC('
        CREATE PROCEDURE stp_GetFavorites
            @UserId INT
        AS
        BEGIN
            SET NOCOUNT ON;

            SELECT RepositoryName, RepositoryUrl
            FROM Favorites
            WHERE UserId = @UserId;
        END;
    ');
    PRINT 'Stored procedure "stp_GetFavorites" created successfully.';
END
ELSE
BEGIN
    PRINT 'Stored procedure "stp_GetFavorites" already exists.';
END;

-- Create login if it does not exist
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'GitHubSearchAppLogin')
BEGIN
    CREATE LOGIN GitHubSearchAppLogin WITH PASSWORD = 'YourSecurePassword123!';
    PRINT 'Login "GitHubSearchAppLogin" created successfully.';
END
ELSE
BEGIN
    PRINT 'Login "GitHubSearchAppLogin" already exists.';
END;

-- Create user in the database for the login
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'GitHubSearchAppUser')
BEGIN
    CREATE USER GitHubSearchAppUser FOR LOGIN GitHubSearchAppLogin;
    PRINT 'User "GitHubSearchAppUser" created successfully.';
END
ELSE
BEGIN
    PRINT 'User "GitHubSearchAppUser" already exists.';
END;

-- Create role and assign permissions
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ProcedureExecutor')
BEGIN
    CREATE ROLE ProcedureExecutor;
    PRINT 'Role "ProcedureExecutor" created successfully.';
END
ELSE
BEGIN
    PRINT 'Role "ProcedureExecutor" already exists.';
END;

-- Assign permissions to the role
GRANT EXECUTE ON SCHEMA::dbo TO ProcedureExecutor;

-- Add user to the role
ALTER ROLE ProcedureExecutor ADD MEMBER GitHubSearchAppUser;

PRINT 'Permissions granted to "GitHubSearchAppUser" via "ProcedureExecutor" role.';
