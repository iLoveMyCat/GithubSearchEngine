using GithubSearchAPI.DTOs;
using GithubSearchAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GithubSearchAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);

            if (response == null)
            {
                return Unauthorized(new { message = "invalid credentials" });
            }

            //I would call the key differently in production something not obvious like "auth-token"
            Response.Cookies.Append("auth-token", response.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, // change to strict for production.
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Ok(new
            {
                Message = "succefully logged in .",
                Username = response.Username
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Logout()
        {
            // preferablly add a token blacklist - delete cookie -> add token to blacklist -> remove expired tokens. and move implementation to the service layer
            Response.Cookies.Delete("auth-token");
            return Ok(new { messsage = "Succefully logged out" });
        }

        [HttpPost]
        [Authorize]
        public IActionResult TestAuthroize()
        {
            return Ok(new { messsage = "You are authorized" });
        }

    }
}
