using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WarehouseAPI.Entities;
using WarehouseAPI.Models;
using WarehouseAPI.Services.Auth;

namespace WarehouseAPI.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        /// <summary>
        /// POST /api/Auth/register - Đăng ký tài khoản mới
        /// 3 Roles có sẵn:
        /// - "Admin" (chỉ người tạo hệ thống)
        /// - "Nhân viên kho" (mặc định nếu không chỉ định)
        /// - "Quản lý kho"
        /// Test: {"username":"user1","password":"Pass123!","role":"Quản lý kho"}
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
                return BadRequest("User already exists.");

            return Ok(user);
        }

        /// <summary>
        /// POST /api/Auth/login - Đăng nhập lấy tokens
        /// Test: {"username":"user1","password":"Pass123!"}
        /// Response: {"accessToken":"...", "refreshToken":"..."}
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
                return BadRequest("Invalid username or password.");
            return Ok(result);
        }

        /// <summary>
        /// POST /api/Auth/refresh-token - Làm mới access token (khi hết hạn)
        /// Test: {"userId":1,"refreshToken":"..."}
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokenAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");
            return Ok(result);
        }

        /// <summary>
        /// GET /api/Auth - Test xem token có hợp lệ không
        /// Cách test: Thêm header Authorization: Bearer [accessToken]
        /// Response (200): "You are authenticated!"
        /// Response (401): Unauthorized nếu token không hợp lệ
        /// </summary>
        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndPoint()
        {
            return Ok("You are authenticated!");
        }

        /// <summary>
        /// GET /api/Auth/admin-only - Chỉ Admin (người tạo hệ thống) có thể access
        /// Cách test: 
        /// 1. Register với role: "Admin" (chỉ có admin mới được cấp role này)
        /// 2. Login lấy accessToken
        /// 3. Thêm header Authorization: Bearer [accessToken]
        /// Response (200): "You are an admin!" nếu role = "Admin"
        /// Response (403): Forbidden nếu role khác (Nhân viên kho, Quản lý kho)
        /// Response (401): Unauthorized nếu không có token
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndPoint()
        {
            return Ok("You are an admin!");
        }
    }
}
