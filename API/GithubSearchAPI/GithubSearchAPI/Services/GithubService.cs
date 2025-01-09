using GithubSearchAPI.DTOs;
using GithubSearchAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GithubSearchAPI.Services
{
    public class GithubService : IGithubService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;


        public GithubService(HttpClient httpClient, IConfiguration configuration)
        {   
            _httpClient = httpClient;
            _configuration= configuration;
        }

        public async Task<IEnumerable<SearchResultDTO>> SearchRepositoriesAsync(string query)
        {
            var requestUrl = $"https://api.github.com/search/repositories?q={query}";

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

                return githubResponse?.Items.Select(repo => new SearchResultDTO
                {
                    RepositoryName = repo.Name,
                    RepositoryUrl = repo.HtmlUrl,
                    Description = repo.Description
                }) ?? Enumerable.Empty<SearchResultDTO>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("failed processing the gitHub api request.", ex);
            }
        }

    }
}
