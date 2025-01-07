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

            //I would call it differently in production something not obvious like Authcookie
            Response.Cookies.Append("auth-token", response.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Ok(new
            {
                Message = "succefully logged in .",
                Username = response.Username
            });
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // preferablly add a token blacklist - delete cookie -> add token to blacklist -> remove expired tokens. and move it to the service
            Response.Cookies.Delete("auth-token");
            return Ok(new { messsage = "Succefully logged out" });
        }
    }
}
