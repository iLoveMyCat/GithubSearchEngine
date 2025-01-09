using GithubSearchAPI.DTOs;
using GithubSearchAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        //[HttpPost]
        //public async Task<IActionResult> AddToFavorites([FromBody] FavoriteDTO favorite)
        //{
        //    return Ok();
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetFavorites()
        //{
        //    return Ok();
        //}
    }
}
