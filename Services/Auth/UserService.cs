using System.Security.Claims;

namespace WarehouseAPI.Services.Auth
{
    public interface IUserService
    {
        string GetUsername(ClaimsPrincipal user);
        int? GetUserId(ClaimsPrincipal user);
        string GetUserRole(ClaimsPrincipal user);
    }

    public class UserService : IUserService
    {
        public string GetUsername(ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
        }

        public int? GetUserId(ClaimsPrincipal user)
        {
            var idStr = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idStr, out var id))
                return id;
            return null;
        }

        public string GetUserRole(ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.Role)?.Value ?? "Nhân viên kho";
        }
    }
}
