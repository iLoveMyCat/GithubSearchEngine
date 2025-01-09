using BCrypt.Net;
using GithubSearchAPI.DTOs;
using GithubSearchAPI.Models;
using GithubSearchAPI.Repositoreis;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GithubSearchAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequest)
        {
            try
            {

                var user = await _authRepository.GetUserByUsernameAsync(loginRequest.Username);

                // Validate password
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.HashedPassword))
                {
                    return null;
                }

                var token = GenerateJwtToken(user.Username, user.Id);

                return new LoginResponseDTO
                {
                    Token = token,
                    Username = user.Username,
                };
            }
            catch (Exception ex)
            {
                // i would add a logger here
                Console.WriteLine($"Error in LoginAsync: {ex.Message}");
                throw new Exception("An error occurred during login. Please try again later.", ex);
            }
        }


        public Task Logout()
        {
            // implementation will move here if we add some logic like creating a token blacklist... currently implemented in the controller
            throw new NotImplementedException();
        }

        private string GenerateJwtToken(string username, int userId)
        {
            try
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Convert.FromBase64String(_configuration["Jwt:SecretKey"]);

                var claims = new[] {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                // I would add a logger here
                Console.WriteLine($"Error in GenerateJwtToken: {ex.Message}");
                throw new Exception("An error occurred while generating the JWT token.", ex);
            }
        }

        public async Task<int> RegisterUserAsync(string username, string password)
        {
            try
            {

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                return await _authRepository.CreateUserAsync(username, hashedPassword);
            }
            catch (Exception ex)
            {
                // I would add a logger here
                Console.WriteLine($"Error in RegisterUserAsync: {ex.Message}");
                throw new Exception("An error occurred during user registration. Please try again later.", ex);
            }
        }
    }
}
