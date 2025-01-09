using GithubSearchAPI.Models;

namespace GithubSearchAPI.Repositoreis
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<int> CreateUserAsync(string username, string hashedPassword);
    }
}
