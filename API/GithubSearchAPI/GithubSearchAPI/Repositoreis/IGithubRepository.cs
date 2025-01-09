using GithubSearchAPI.DTOs;

namespace GithubSearchAPI.Repositoreis
{
    public interface IGithubRepository
    {
        Task AddFavoriteAsync(FavoriteDTO favorite, int userId);
        Task<IEnumerable<FavoriteDTO>> GetFavoritesAsync(int userId);
    }
}
