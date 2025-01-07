using GithubSearchAPI.Models;

namespace GithubSearchAPI.Repositoreis
{
    public interface IAuthRepository
    {
        Task<User> ValidateUserAsync(string username, string hashedPassword); 
    }
}
