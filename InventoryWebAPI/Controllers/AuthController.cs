using InventoryWebAPI.Application.DTOs.Auth;
using InventoryWebAPI.Application.Interfaces;
using InventoryWebAPI.Domain.Entities;
using InventoryWebAPI.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace InventoryWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;

        public AuthController(IJwtService jwtService, IUnitOfWork uow, IConfiguration config)
        {
            _jwtService = jwtService;
            _uow = uow;
            _config = config;
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
            var user = await _uow.Users.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("Invalid username or password");

            var validPassword = await _uow.Users.CheckPasswordAsync(user, model.Password);
            if (!validPassword) return Unauthorized("Invalid username or password");

            return await CreateApplicationUserDto(user);
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

            return Ok(new { title = "Account Created", message = "Your account has been created." });
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
        #endregion
    }
}