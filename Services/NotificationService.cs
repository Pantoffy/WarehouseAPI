using WarehouseAPI.Models;
using WarehouseAPI.Repository;
using WarehouseAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace WarehouseAPI.Services
{
    public class NotificationDto
    {
        public string Key { get; set; } = "";   // stable: "{TYPE}_{entityId}", e.g. "IMPORT_12"
        public int Id { get; set; }
        public string Type { get; set; } = "";  // IMPORT | EXPORT | PURCHASE_ORDER | STOCK_CHECK | LOW_STOCK
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string TargetUrl { get; set; } = ""; // route FE navigates to on click
        public DateTime CreatedAt { get; set; }
        public string Priority { get; set; } = "normal"; // normal | high | critical
    }

    public interface INotificationService
    {
        Task<List<NotificationDto>> GetNotificationsAsync(string? userRole = null);
    }

    public class NotificationService : INotificationService
    {
        private readonly IUOW _uow;
        private readonly AppDbContext _context;

        public NotificationService(IUOW uow, AppDbContext context)
        {
            _uow = uow;
            _context = context;
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(string? userRole = null)
        {
            var notifications = new List<NotificationDto>();
            const string pendingStatus = "Chờ xác nhận";

            // 1. Low stock notifications (all users)
            var inventories = await _context.Inventory.Include(i => i.Material).ToListAsync();
            const int lowStockThreshold = 10;

            foreach (var inv in inventories ?? new List<Models.Inventory>())
            {
                if (inv.Quantity < lowStockThreshold)
                {
                    notifications.Add(new NotificationDto
                    {
                        Key = $"LOW_STOCK_{inv.Id}",
                        Id = inv.Id,
                        Type = "LOW_STOCK",
                        Title = inv.Quantity == 0
                            ? $"Hết hàng: {inv.Material?.Name ?? "Vật tư"}"
                            : $"Sắp hết: {inv.Material?.Name ?? "Vật tư"}",
                        Message = inv.Quantity == 0
                            ? $"{inv.Material?.Name ?? "Vật tư"} đã hết hàng trong kho."
                            : $"{inv.Material?.Name ?? "Vật tư"} chỉ còn {inv.Quantity} — cần nhập thêm.",
                        TargetUrl = "/ton-kho-theo-kho",
                        CreatedAt = DateTime.UtcNow,
                        Priority = inv.Quantity == 0 ? "critical" : "high"
                    });
                }
            }

            // 2. Pending receipts (only for warehouse manager)
            if (userRole == "Quản lý kho")
            {
                var importReceipts = await _context.ImportReceipt
                    .Include(r => r.Supplier)
                    .Where(r => r.Status == pendingStatus)
                    .ToListAsync();

                foreach (var receipt in importReceipts)
                {
                    notifications.Add(new NotificationDto
                    {
                        Key = $"IMPORT_{receipt.Id}",
                        Id = receipt.Id,
                        Type = "IMPORT",
                        Title = $"Phiếu nhập {receipt.ReceiptNumber} chờ xác nhận",
                        Message = $"Phiếu nhập từ {receipt.Supplier?.Name ?? "nhà cung cấp"} cần được xác nhận.",
                        TargetUrl = $"/nhap-kho?id={receipt.Id}",
                        CreatedAt = receipt.CreatedAt,
                        Priority = "normal"
                    });
                }

                var exportReceipts = await _context.ExportReceipt
                    .Where(r => r.Status == pendingStatus)
                    .ToListAsync();

                foreach (var receipt in exportReceipts)
                {
                    notifications.Add(new NotificationDto
                    {
                        Key = $"EXPORT_{receipt.Id}",
                        Id = receipt.Id,
                        Type = "EXPORT",
                        Title = $"Phiếu xuất {receipt.ReceiptNumber} chờ xác nhận",
                        Message = $"Phiếu xuất cho {receipt.ReceiverName ?? "người nhận"} cần được xác nhận.",
                        TargetUrl = $"/xuat-kho?id={receipt.Id}",
                        CreatedAt = receipt.CreatedAt,
                        Priority = "normal"
                    });
                }

                var stockChecks = await _context.StockCheck
                    .Include(s => s.Warehouse)
                    .Where(s => s.Status == "Đã trình")
                    .ToListAsync();

                foreach (var check in stockChecks)
                {
                    var label = !string.IsNullOrEmpty(check.Code) ? check.Code : $"#{check.Id}";
                    notifications.Add(new NotificationDto
                    {
                        Key = $"STOCK_CHECK_{check.Id}",
                        Id = check.Id,
                        Type = "STOCK_CHECK",
                        Title = $"Phiếu kiểm {label} chờ duyệt",
                        Message = $"{check.Name ?? label} — {check.Warehouse?.Name ?? "kho"}",
                        TargetUrl = $"/kiem-ke/chi-tiet/{check.Id}",
                        CreatedAt = check.CreatedTime,
                        Priority = "normal"
                    });
                }
            }

            // 3. Approaching PO delivery dates (all users)
            var purchaseOrders = await _context.PurchaseOrder.Include(po => po.Supplier).ToListAsync();
            var now = DateTime.UtcNow;
            var threeDaysLater = now.AddDays(3);

            foreach (var po in purchaseOrders.Where(po =>
                po.ExpectedDeliveryDate.HasValue &&
                po.ExpectedDeliveryDate.Value >= now &&
                po.ExpectedDeliveryDate.Value <= threeDaysLater))
            {
                var days = (po.ExpectedDeliveryDate!.Value - now).Days;
                notifications.Add(new NotificationDto
                {
                    Key = $"PURCHASE_ORDER_{po.Id}",
                    Id = po.Id,
                    Type = "PURCHASE_ORDER",
                    Title = days == 0
                        ? $"Đơn hàng {po.PoNumber} giao hôm nay"
                        : $"Đơn hàng {po.PoNumber} giao trong {days} ngày",
                    Message = $"Đơn từ {po.Supplier?.Name ?? "nhà cung cấp"} — hạn giao {po.ExpectedDeliveryDate.Value:dd/MM/yyyy}.",
                    TargetUrl = $"/don-dat-hang?id={po.Id}",
                    CreatedAt = po.CreatedAt,
                    Priority = days == 0 ? "critical" : "high"
                });
            }

            var priorityOrder = new Dictionary<string, int>
            {
                { "critical", 0 }, { "high", 1 }, { "normal", 2 }, { "low", 3 }
            };

            return notifications
                .OrderBy(n => priorityOrder.GetValueOrDefault(n.Priority, 2))
                .ThenByDescending(n => n.CreatedAt)
                .ToList();
        }
    }
}
