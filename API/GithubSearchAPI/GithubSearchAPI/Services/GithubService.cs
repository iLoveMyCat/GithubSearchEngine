using GithubSearchAPI.DTOs;
using GithubSearchAPI.Models;
using GithubSearchAPI.Repositoreis;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GithubSearchAPI.Services
{
    public class GithubService : IGithubService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IGithubRepository _githubRepository;
        private readonly IMemoryCache _cache;

        public GithubService(HttpClient httpClient, IConfiguration configuration, IGithubRepository githubRepository, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _githubRepository = githubRepository;
            _cache = cache;
        }

        public async Task<IEnumerable<SearchResultDTO>> SearchRepositoriesAsync(string query, int? limit = null)
        {
            var cacheKey = $"Search_{query}_{limit ?? 30}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<SearchResultDTO> cachedResults))
            {
                return cachedResults;  
            }

            var perPage = limit ?? 30;
            var requestUrl = $"https://api.github.com/search/repositories?q={query}&per_page={perPage}";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("User-Agent", "MyApp/1.0");

                var token = _configuration["GitHub:Token"];
                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new InvalidOperationException("GitHub token is missing or not configured.");
                }
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Send request
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"GitHub API returned an error: {response.StatusCode} - {response.ReasonPhrase}");
                }

                // Deserialize 
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var githubResponse = JsonSerializer.Deserialize<GitHubSearchResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var results = githubResponse?.Items.Select(repo => new SearchResultDTO
                {
                    RepositoryName = repo.Name,
                    RepositoryUrl = repo.HtmlUrl,
                    Description = repo.Description
                }) ?? Enumerable.Empty<SearchResultDTO>();

                // cache expiration of 5 minutes
                _cache.Set(cacheKey, results, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                });

                return results;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("failed processing the gitHub api request.", ex);
            }
        }

        public async Task AddToFavoritesAsync(FavoriteDTO favorite, int userId)
        {
            if (favorite == null || userId <= 0)
            {
                throw new ArgumentException("Invalid data provided.");
            }

            await _githubRepository.AddFavoriteAsync(favorite, userId);
        }

        public async Task<IEnumerable<FavoriteDTO>> GetFavoritesAsync(int userId)
        {
            return await _githubRepository.GetFavoritesAsync(userId);
        }



    }
}
