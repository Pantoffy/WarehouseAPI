using WarehouseAPI.DTOs.CalendarDTOs;
using WarehouseAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace WarehouseAPI.Services.Calendar
{
    public interface ICalendarService
    {
        Task<List<CalendarEventResponse>> GetEventsAsync(int? month = null, int? year = null);
        Task<List<CalendarEventResponse>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }

    public class CalendarService : ICalendarService
    {
        private readonly AppDbContext _context;

        public CalendarService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy tất cả event từ 4 bảng: ImportReceipt, ExportReceipt, PurchaseOrder, StockCheck
        /// Có thể filter theo tháng/năm
        /// </summary>
        public async Task<List<CalendarEventResponse>> GetEventsAsync(int? month = null, int? year = null)
        {
            var events = new List<CalendarEventResponse>();

            var startDate = month.HasValue && year.HasValue
                ? new DateTime(year.Value, month.Value, 1)
                : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Lấy ImportReceipt
            var importReceipts = await _context.ImportReceipt
                .Include(ir => ir.Warehouse)
                .Where(ir => ir.ImportTime >= startDate && ir.ImportTime <= endDate)
                .Where(ir => ir.Status == "Đã duyệt" || ir.Status == "Hoàn thành")
                .ToListAsync();

            foreach (var import in importReceipts)
            {
                events.Add(new CalendarEventResponse
                {
                    Id = import.Id,
                    Title = $"Nhập kho - {import.Code}",
                    Date = import.ImportTime,
                    Type = "import",
                    Color = "green",
                    Status = import.Status,
                    Amount = import.TotalAmount,
                    WarehouseName = import.Warehouse?.Name,
                    Code = import.Code,
                    DetailUrl = $"/nhap-kho/{import.Id}"
                });
            }

            // Lấy ExportReceipt
            var exportReceipts = await _context.ExportReceipt
                .Include(er => er.Warehouse)
                .Where(er => er.ExportDate >= startDate && er.ExportDate <= endDate)
                .Where(er => er.Status == "Đã duyệt" || er.Status == "Hoàn thành")
                .ToListAsync();

            foreach (var export in exportReceipts)
            {
                events.Add(new CalendarEventResponse
                {
                    Id = export.Id,
                    Title = $"Xuất kho - {export.Code}",
                    Date = export.ExportDate,
                    Type = "export",
                    Color = "red",
                    Status = export.Status,
                    Amount = export.TotalAmount,
                    WarehouseName = export.Warehouse?.Name,
                    Code = export.Code,
                    DetailUrl = $"/xuat-kho/{export.Id}"
                });
            }

            // Lấy PurchaseOrder
            var purchaseOrders = await _context.PurchaseOrder
                .Where(po => po.OrderDate >= startDate && po.OrderDate <= endDate)
                .Where(po => po.Status == "Đã duyệt" || po.Status == "Hoàn thành")
                .ToListAsync();

            foreach (var po in purchaseOrders)
            {
                events.Add(new CalendarEventResponse
                {
                    Id = po.Id,
                    Title = $"PO - {po.Code}",
                    Date = po.OrderDate,
                    Type = "po",
                    Color = "blue",
                    Status = po.Status,
                    Amount = po.TotalAmount,
                    Code = po.Code,
                    DetailUrl = $"/purchase-order/{po.Id}"
                });
            }

            // Lấy StockCheck
            var stockChecks = await _context.StockCheck
                .Include(sc => sc.Warehouse)
                .Where(sc => sc.CheckTime != null && sc.CheckTime >= startDate && sc.CheckTime <= endDate)
                .Where(sc => sc.Status == "Đã duyệt" || sc.Status == "Hoàn thành")
                .ToListAsync();

            foreach (var stock in stockChecks)
            {
                events.Add(new CalendarEventResponse
                {
                    Id = stock.Id,
                    Title = $"Kiểm kê - {stock.Code}",
                    Date = stock.CheckTime.HasValue ? stock.CheckTime.Value : stock.CreatedTime,
                    Type = "stockcheck",
                    Color = "yellow",
                    Status = stock.Status,
                    WarehouseName = stock.Warehouse?.Name,
                    Code = stock.Code,
                    DetailUrl = $"/kiem-ke/chi-tiet/{stock.Id}"
                });
            }

            // Sắp xếp theo ngày
            return events.OrderBy(e => e.Date).ToList();
        }

        /// <summary>
        /// Lấy event trong khoảng thời gian cụ thể
        /// </summary>
        public async Task<List<CalendarEventResponse>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var events = new List<CalendarEventResponse>();

            // Lấy ImportReceipt
            var importReceipts = await _context.ImportReceipt
                .Include(ir => ir.Warehouse)
                .Where(ir => ir.ImportTime >= startDate && ir.ImportTime <= endDate)
                .Where(ir => ir.Status == "Đã duyệt" || ir.Status == "Hoàn thành")
                .ToListAsync();

            foreach (var import in importReceipts)
            {
                events.Add(new CalendarEventResponse
                {
                    Id = import.Id,
                    Title = $"Nhập kho - {import.Code}",
                    Date = import.ImportTime,
                    Type = "import",
                    Color = "green",
                    Status = import.Status,
                    Amount = import.TotalAmount,
                    WarehouseName = import.Warehouse?.Name,
                    Code = import.Code,
                    DetailUrl = $"/nhap-kho/{import.Id}"
                });
            }

            // Lấy ExportReceipt
            var exportReceipts = await _context.ExportReceipt
                .Include(er => er.Warehouse)
                .Where(er => er.ExportDate >= startDate && er.ExportDate <= endDate)
                .Where(er => er.Status == "Đã duyệt" || er.Status == "Hoàn thành")
                .ToListAsync();

            foreach (var export in exportReceipts)
            {
                events.Add(new CalendarEventResponse
                {
                    Id = export.Id,
                    Title = $"Xuất kho - {export.Code}",
                    Date = export.ExportDate,
                    Type = "export",
                    Color = "red",
                    Status = export.Status,
                    Amount = export.TotalAmount,
                    WarehouseName = export.Warehouse?.Name,
                    Code = export.Code,
                    DetailUrl = $"/xuat-kho/{export.Id}"
                });
            }

            // Lấy PurchaseOrder
            var purchaseOrders = await _context.PurchaseOrder
                .Where(po => po.OrderDate >= startDate && po.OrderDate <= endDate)
                .Where(po => po.Status == "Đã duyệt" || po.Status == "Hoàn thành")
                .ToListAsync();

            foreach (var po in purchaseOrders)
            {
                events.Add(new CalendarEventResponse
                {
                    Id = po.Id,
                    Title = $"PO - {po.Code}",
                    Date = po.OrderDate,
                    Type = "po",
                    Color = "blue",
                    Status = po.Status,
                    Amount = po.TotalAmount,
                    Code = po.Code,
                    DetailUrl = $"/purchase-order/{po.Id}"
                });
            }

            // Lấy StockCheck
            var stockChecks = await _context.StockCheck
                .Include(sc => sc.Warehouse)
                .Where(sc => sc.CheckTime != null && sc.CheckTime >= startDate && sc.CheckTime <= endDate)
                .Where(sc => sc.Status == "Đã duyệt" || sc.Status == "Hoàn thành")
                .ToListAsync();

            foreach (var stock in stockChecks)
            {
                events.Add(new CalendarEventResponse
                {
                    Id = stock.Id,
                    Title = $"Kiểm kê - {stock.Code}",
                    Date = stock.CheckTime.HasValue ? stock.CheckTime.Value : stock.CreatedTime,
                    Type = "stockcheck",
                    Color = "yellow",
                    Status = stock.Status,
                    WarehouseName = stock.Warehouse?.Name,
                    Code = stock.Code,
                    DetailUrl = $"/kiem-ke/chi-tiet/{stock.Id}"
                });
            }

            // Sắp xếp theo ngày
            return events.OrderBy(e => e.Date).ToList();
        }
    }
}
