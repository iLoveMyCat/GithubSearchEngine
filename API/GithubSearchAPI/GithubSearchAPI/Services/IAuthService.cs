using GithubSearchAPI.DTOs;

namespace GithubSearchAPI.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest);
        Task Logout();
    }
}
