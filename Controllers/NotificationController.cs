using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WarehouseAPI.Services;
using WarehouseAPI.Services.Auth;

namespace WarehouseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public NotificationController(INotificationService notificationService, IUserService userService)
        {
            _notificationService = notificationService;
            _userService = userService;
        }

        /// <summary>
        /// GET /api/Notification - Get list of notifications based on user role
        /// Returns:
        /// - Low stock warnings (for all users)
        /// - Pending approvals (only for "Quản lý kho" role)
        /// - Approaching delivery dates for POs (for all users)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<NotificationDto>>> GetNotifications()
        {
            try
            {
                var userRole = _userService.GetUserRole(User);
                var notifications = await _notificationService.GetNotificationsAsync(userRole);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
