using GithubSearchAPI.DTOs;
using System.Data.SqlClient;
using System.Data;
using GithubSearchAPI.Models;
using System.Collections.Generic;

namespace GithubSearchAPI.Repositoreis
{
    public class GithubRepository : IGithubRepository
    {
        private readonly string _connectionString;

        public GithubRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //    CREATE PROCEDURE stp_AddFavorite
        //      @FavoriteId INT OUTPUT,
        //      @UserId INT,
        //      @RepositoryName NVARCHAR(255),
        //      @RepositoryUrl NVARCHAR(500)
        //    AS
        //    BEGIN
        //      INSERT INTO Favorites(UserId, RepositoryName, RepositoryUrl, CreatedAt)
        //      VALUES(@UserId, @RepositoryName, @RepositoryUrl, GETDATE());

        //      SELECT @FavoriteId = SCOPE_IDENTITY();
        //    END;
        //GO
        public async Task AddFavoriteAsync(FavoriteDTO favorite, int userId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("stp_AddFavorite", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@FavoriteId", DBNull.Value); 
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@RepositoryName", favorite.RepositoryName);
                command.Parameters.AddWithValue("@RepositoryUrl", favorite.RepositoryUrl);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding favorite to the database.", ex);
            }
        }


        //    CREATE PROCEDURE stp_GetFavorites
        //      @UserId INT
        //    AS
        //    BEGIN
        //      SELECT RepositoryName, RepositoryUrl
        //      FROM Favorites
        //      WHERE UserId = @UserId;
        //    END;

        public async Task<IEnumerable<FavoriteDTO>> GetFavoritesAsync(int userId)
        {
            var favorites = new List<FavoriteDTO>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("stp_GetFavorites", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                favorites.Add(new FavoriteDTO
                {
                    RepositoryName = reader.GetString(reader.GetOrdinal("RepositoryName")),
                    RepositoryUrl = reader.GetString(reader.GetOrdinal("RepositoryUrl"))
                });
            }

            return favorites;
        }

    }
}
