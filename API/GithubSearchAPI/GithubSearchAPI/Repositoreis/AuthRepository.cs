using GithubSearchAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GithubSearchAPI.Repositoreis
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //    CREATE PROCEDURE stp_validateUser
        //    @Username NVARCHAR(50),
        //    @PasswordHash NVARCHAR(100)
        //    AS
        //    BEGIN
        //      SELECT Id, Username
        //      FROM Users
        //      WHERE Username = @Username AND PasswordHash = @PasswordHash;
        //    END
        public async Task<User> ValidateUserAsync(string username, string hashedPassword)
        {
            //if(username == "user13" )
            //{
            //    return new User
            //    {
            //        Id = 13,
            //        Username = "user13"
            //    };
            //}
            //else
            //{
            //    return null;
            //}

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("stp_validateUser", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32("Id"),
                    Username = reader.GetString("Username")
                };
            }

            return null;
        }
    }
}
