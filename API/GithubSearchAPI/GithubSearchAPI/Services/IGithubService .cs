﻿using GithubSearchAPI.DTOs;
using GithubSearchAPI.Models;

namespace GithubSearchAPI.Services
{
    public interface IGithubService
    {
        Task<IEnumerable<SearchResultDTO>> SearchRepositoriesAsync(string query, int? limit);
        Task AddToFavoritesAsync(FavoriteDTO favorite, int userId);
        Task<IEnumerable<FavoriteDTO>> GetFavoritesAsync(int userId);

    }
}
