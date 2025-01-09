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
            catch (SqlException ex)
            {
                throw new Exception("Error adding favorite to the database.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding the favorite.", ex);
            }
        }

        public async Task<IEnumerable<FavoriteDTO>> GetFavoritesAsync(int userId)
        {
            var favorites = new List<FavoriteDTO>();
            try
            {


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

            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving favorites from the database.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching favorites.", ex);
            }
            return favorites;
        }

    }
}
