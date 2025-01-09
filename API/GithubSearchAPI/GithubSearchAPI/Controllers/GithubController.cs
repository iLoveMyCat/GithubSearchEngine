using GithubSearchAPI.DTOs;
using GithubSearchAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GithubSearchAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class GithubController : Controller
    {
        private readonly IGithubService _gitHubService;

        public GithubController(IGithubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { message = "Query cannot be empty" });

            var results = await _gitHubService.SearchRepositoriesAsync(query);
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites([FromBody] FavoriteDTO favorite)
        {
            if (favorite == null || string.IsNullOrWhiteSpace(favorite.RepositoryName) || string.IsNullOrWhiteSpace(favorite.RepositoryUrl))
            {
                return BadRequest(new { message = "Invalid favorite data" });
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            await _gitHubService.AddToFavoritesAsync(favorite, userId.Value);
            return Ok(new { message = "Favorite added successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var favorites = await _gitHubService.GetFavoritesAsync(userId.Value);
            return Ok(favorites);
        }



        protected int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }
    }


}
