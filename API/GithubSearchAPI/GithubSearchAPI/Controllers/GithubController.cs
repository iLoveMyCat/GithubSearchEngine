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
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest(new { message = "Query cannot be empty" });

                var results = await _gitHubService.SearchRepositoriesAsync(query);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the request.", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites([FromBody] FavoriteDTO favorite)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding to favorites.", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            try
            {

                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized();

                var favorites = await _gitHubService.GetFavoritesAsync(userId.Value);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching favorites.", details = ex.Message });
            }
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
