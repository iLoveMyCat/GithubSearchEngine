-- Declare a variable for the password
DECLARE @Password NVARCHAR(100);

-- Set the password dynamically
-- Replace 'TemporaryPassword123!' with a real password when executing
SET @Password = 'TemporaryPassword123!';
 
-- Create the database
CREATE DATABASE GITHUB_SEARCH;
GO

-- Switch to the database
USE GITHUB_SEARCH;
GO

-- Create Users table
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(100) NOT NULL
);
GO

-- Create Favorites table
CREATE TABLE Favorites (
    FavoriteId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    RepositoryName NVARCHAR(255) NOT NULL,
    RepositoryUrl NVARCHAR(500) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

-- Create stored procedures
CREATE PROCEDURE stp_validateUser
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(100)
AS
BEGIN
    SELECT Id, Username
    FROM Users
    WHERE Username = @Username AND PasswordHash = @PasswordHash;
END;
GO

CREATE PROCEDURE stp_AddFavorite
    @FavoriteId INT OUTPUT, -- Auto-incremented by DB
    @UserId INT,
    @RepositoryName NVARCHAR(255),
    @RepositoryUrl NVARCHAR(500)
AS
BEGIN
    INSERT INTO Favorites (UserId, RepositoryName, RepositoryUrl, CreatedAt)
    VALUES (@UserId, @RepositoryName, @RepositoryUrl, GETDATE());

    SELECT @FavoriteId = SCOPE_IDENTITY(); -- Return the new ID
END;
GO

CREATE PROCEDURE stp_GetFavorites
    @UserId INT
AS
BEGIN
    SELECT RepositoryName, RepositoryUrl
    FROM Favorites
    WHERE UserId = @UserId;
END;
GO

-- Create login
EXEC('CREATE LOGIN GitHubSearchAppLogin WITH PASSWORD = ''{PASSWORD}''');
GO

-- Create user for the login
CREATE USER GitHubSearchAppUser 
FOR LOGIN GitHubSearchAppLogin;
GO

-- Create a role for procedure execution
CREATE ROLE ProcedureExecutor;
GRANT EXECUTE TO ProcedureExecutor;
DENY SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO ProcedureExecutor;
ALTER ROLE ProcedureExecutor ADD MEMBER GitHubSearchAppUser;
GO
