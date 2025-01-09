using GithubSearchAPI.DTOs;
using GithubSearchAPI.Models;

namespace GithubSearchAPI.Services
{
    public interface IGithubService
    {
        Task<IEnumerable<SearchResultDTO>> SearchRepositoriesAsync(string query);
    }
}
