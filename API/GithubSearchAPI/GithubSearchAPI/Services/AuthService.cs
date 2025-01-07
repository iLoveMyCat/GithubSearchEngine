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
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginRequest.Password);

            var user = await _authRepository.ValidateUserAsync(loginRequest.Username, hashedPassword);

            if (user == null) return null;

            var token = GenerateJwtToken(user.Username);

            return new LoginResponseDTO {
                Token = token,
                Username = user.Username,
            };
        }

        public Task Logout()
        {
            // implementation will move here if we add some logic like creating a token blacklist... currently implemented in the controller
            throw new NotImplementedException();
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_configuration["Jwt:SecretKey"]);



            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
