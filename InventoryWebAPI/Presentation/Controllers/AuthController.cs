using InventoryWebAPI.Application.DTOs.Auth;
using InventoryWebAPI.Application.Interfaces;
using InventoryWebAPI.Domain.Entities;
using InventoryWebAPI.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using InventoryWebAPI.Hubs;

namespace InventoryWebAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;
        private readonly IHubContext<ProgressHub> _progressHubContext; // নতুন যোগ করো

        public AuthController(IJwtService jwtService, IUnitOfWork uow, IConfiguration config, IHubContext<ProgressHub> progressHubContext)
        {
            _jwtService = jwtService;
            _uow = uow;
            _config = config;
            _progressHubContext = progressHubContext; // নতুন যোগ করো
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDto>> RefereshToken()
        {
            var token = Request.Cookies[_config["JWT:CookiesKey"]];
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (await IsValidRefreshTokenAsync(userId, token))
            {
                var user = await _uow.Users.FindByIdAsync(userId);
                if (user == null) return Unauthorized("Invalid or expired token, please try to login");
                return await CreateApplicationUserDto(user);
            }
            return Unauthorized("Invalid or expired token, please try to login");
        }

        [Authorize]
        [HttpGet("refresh-page")]
        public async Task<ActionResult<UserDto>> RefreshPage()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();
            var user = await _uow.Users.FindByEmailAsync(email);
            if (user == null) return Unauthorized();
            return await CreateApplicationUserDto(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            // ক্লায়েন্টের কানেকশন ID পাওয়া (ফ্রন্টএন্ড থেকে পাঠানো হবে)
            var connectionId = Request.Query["connectionId"]; // ফ্রন্টএন্ড থেকে কুয়েরি প্যারামিটার হিসেবে পাঠাও

            if (string.IsNullOrEmpty(connectionId))
                return BadRequest("Connection ID required for real-time progress");

            // প্রোগ্রেস সিমুলেশন শুরু
            await SendProgress(connectionId, 0);

            var user = await _uow.Users.FindByEmailAsync(model.Email);
            if (user == null)
            {
                await SendProgress(connectionId, 100); // সম্পূর্ণ, কিন্তু এরর
                return Unauthorized("Invalid username or password");
            }
            await SendProgress(connectionId, 20); // 20% সম্পূর্ণ (ইমেইল চেক)

            var validPassword = await _uow.Users.CheckPasswordAsync(user, model.Password);
            if (!validPassword)
            {
                await SendProgress(connectionId, 100);
                return Unauthorized("Invalid username or password");
            }
            await SendProgress(connectionId, 50);

            
            var dto = await CreateApplicationUserDto(user);
            await SendProgress(connectionId, 80);

            await SendProgress(connectionId, 100);
            return dto;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (await _uow.Users.EmailExistsAsync(model.Email))
            {
                return BadRequest($"An existing account is using {model.Email}, email address.");
            }
            var userToAdd = new User
            {
                UserName = model.Username,
                Email = model.Email.ToLower(),
            };
            var success = await _uow.Users.CreateUserAsync(userToAdd, model.Password);
            if (!success) return BadRequest("User creation failed");
            return Ok(new JsonResult(new { title = "Account Created", message = "Your account has been created" }));
        }

        #region Private Helper Methods
        private async Task<UserDto> CreateApplicationUserDto(User user)
        {
            await SaveRefreshTokenAsync(user);
            return new UserDto
            {
                UserName = user.UserName ?? string.Empty,
                JWT = await _jwtService.CreateJWT(user),
            };
        }

        private async Task SaveRefreshTokenAsync(User user)
        {
            var refreshToken = _jwtService.CreateRefreshToken(user);
            var existingRefreshToken = await _uow.Users.GetRefreshTokenByUserIdAsync(user.Id);
            if (existingRefreshToken != null)
            {
                existingRefreshToken.Token = refreshToken.Token;
                existingRefreshToken.DateCreatedUtc = refreshToken.DateCreatedUtc;
                existingRefreshToken.DateExpiresUtc = refreshToken.DateExpiresUtc;
            }
            else
            {
                await _uow.Users.AddRefreshTokenAsync(refreshToken);
            }
            await _uow.CommitAsync();
            var cookieOptions = new CookieOptions
            {
                Expires = refreshToken.DateExpiresUtc,
                IsEssential = true,
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };
            Response.Cookies.Append(_config["JWT:CookiesKey"], refreshToken.Token, cookieOptions);
        }

        private async Task<bool> IsValidRefreshTokenAsync(string? userId, string? token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return false;
            var fetchedRefreshToken = await _uow.Users.GetRefreshTokenAsync(userId, token);
            if (fetchedRefreshToken == null) return false;
            if (fetchedRefreshToken.IsExpired) return false;
            return true;
        }

        private async Task SendProgress(string connectionId, int progress)
        {
            await _progressHubContext.Clients.Client(connectionId).SendAsync("ReceiveProgress", progress);
        }
        #endregion
    }
}