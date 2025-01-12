using GithubSearchAPI.DTOs;
using GithubSearchAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

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

        [HttpGet]
        public IActionResult Session()
        {
            var user = HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                return Ok(new
                {
                    isAuthenticated = true,
                    username = user.Identity.Name
                });
            }

            return Unauthorized(new { isAuthenticated = false });
        }

        // for future refresh token mechanism
        //[HttpPost] 
        //public IActionResult RefreshToken()
        //{
        //    var refreshToken = Request.Cookies["refresh-token"];
        //    if (IsValid(refreshToken))
        //    {
        //        var newToken = GenerateJwtToken();
        //        Response.Cookies.Append("auth-token", newToken, new CookieOptions
        //        {
        //            HttpOnly = true,
        //            Secure = true,
        //            SameSite = SameSiteMode.Lax,
        //            Expires = DateTime.UtcNow.AddMinutes(15)
        //        });

        //        return Ok(new { message = "Token refreshed" });
        //    }

        //    return Unauthorized();
        //}

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
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid input data." });
            }

            try
            {
                var userId = await _authService.RegisterUserAsync(request.Username, request.Password);
                return Ok(new { Message = "User registered successfully.", UserId = userId });
            }
            catch (Exception ex)
            {
                // Log unexpected error
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }


    }
}
