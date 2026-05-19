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
        /// Nếu có month/year thì filter theo tháng, ngược lại lấy tất cả
        /// </summary>
        public async Task<List<CalendarEventResponse>> GetEventsAsync(int? month = null, int? year = null)
        {
            var events = new List<CalendarEventResponse>();

            DateTime? startDate = null;
            DateTime? endDate = null;
            if (month.HasValue && year.HasValue)
            {
                startDate = new DateTime(year.Value, month.Value, 1);
                endDate = startDate.Value.AddMonths(1).AddTicks(-1);
            }

            // ImportReceipt
            var importQuery = _context.ImportReceipt.Include(ir => ir.Warehouse).AsQueryable();
            if (startDate.HasValue && endDate.HasValue)
                importQuery = importQuery.Where(ir => ir.ImportTime >= startDate.Value && ir.ImportTime <= endDate.Value);

            var importReceipts = await importQuery.ToListAsync();
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

            // ExportReceipt
            var exportQuery = _context.ExportReceipt.Include(er => er.Warehouse).AsQueryable();
            if (startDate.HasValue && endDate.HasValue)
                exportQuery = exportQuery.Where(er => er.ExportDate >= startDate.Value && er.ExportDate <= endDate.Value);

            var exportReceipts = await exportQuery.ToListAsync();
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

            // PurchaseOrder: ưu tiên ExpectedDeliveryDate, fallback OrderDate
            var poQuery = _context.PurchaseOrder.AsQueryable();
            if (startDate.HasValue && endDate.HasValue)
                poQuery = poQuery.Where(po => (po.ExpectedDeliveryDate ?? po.OrderDate) >= startDate.Value && (po.ExpectedDeliveryDate ?? po.OrderDate) <= endDate.Value);

            var purchaseOrders = await poQuery.ToListAsync();
            foreach (var po in purchaseOrders)
            {
                var poDate = po.ExpectedDeliveryDate ?? po.OrderDate;
                events.Add(new CalendarEventResponse
                {
                    Id = po.Id,
                    Title = $"PO - {po.Code}",
                    Date = poDate,
                    Type = "po",
                    Color = "blue",
                    Status = po.Status,
                    Amount = po.TotalAmount,
                    Code = po.Code,
                    DetailUrl = $"/purchase-order/{po.Id}"
                });
            }

            // StockCheck: ưu tiên CheckTime, fallback EndDate, StartDate, CreatedTime
            var stockQuery = _context.StockCheck.Include(sc => sc.Warehouse).AsQueryable();
            if (startDate.HasValue && endDate.HasValue)
                stockQuery = stockQuery.Where(sc => (sc.CheckTime ?? sc.EndDate ?? sc.StartDate ?? sc.CreatedTime) >= startDate.Value
                                                  && (sc.CheckTime ?? sc.EndDate ?? sc.StartDate ?? sc.CreatedTime) <= endDate.Value);

            var stockChecks = await stockQuery.ToListAsync();
            foreach (var stock in stockChecks)
            {
                var stockDate = stock.CheckTime ?? stock.EndDate ?? stock.StartDate ?? stock.CreatedTime;
                events.Add(new CalendarEventResponse
                {
                    Id = stock.Id,
                    Title = $"Kiểm kê - {stock.Code}",
                    Date = stockDate,
                    Type = "stockcheck",
                    Color = "yellow",
                    Status = stock.Status,
                    WarehouseName = stock.Warehouse?.Name,
                    Code = stock.Code,
                    DetailUrl = $"/kiem-ke/chi-tiet/{stock.Id}"
                });
            }

            return events.OrderBy(e => e.Date).ToList();
        }

        /// <summary>
        /// Lấy event trong khoảng thời gian cụ thể
        /// </summary>
        public async Task<List<CalendarEventResponse>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var events = new List<CalendarEventResponse>();

            var importReceipts = await _context.ImportReceipt
                .Include(ir => ir.Warehouse)
                .Where(ir => ir.ImportTime >= startDate && ir.ImportTime <= endDate)
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

            var exportReceipts = await _context.ExportReceipt
                .Include(er => er.Warehouse)
                .Where(er => er.ExportDate >= startDate && er.ExportDate <= endDate)
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

            var purchaseOrders = await _context.PurchaseOrder
                .Where(po => (po.ExpectedDeliveryDate ?? po.OrderDate) >= startDate && (po.ExpectedDeliveryDate ?? po.OrderDate) <= endDate)
                .ToListAsync();

            foreach (var po in purchaseOrders)
            {
                var poDate = po.ExpectedDeliveryDate ?? po.OrderDate;
                events.Add(new CalendarEventResponse
                {
                    Id = po.Id,
                    Title = $"PO - {po.Code}",
                    Date = poDate,
                    Type = "po",
                    Color = "blue",
                    Status = po.Status,
                    Amount = po.TotalAmount,
                    Code = po.Code,
                    DetailUrl = $"/purchase-order/{po.Id}"
                });
            }

            var stockChecks = await _context.StockCheck
                .Include(sc => sc.Warehouse)
                .Where(sc => (sc.CheckTime ?? sc.EndDate ?? sc.StartDate ?? sc.CreatedTime) >= startDate
                          && (sc.CheckTime ?? sc.EndDate ?? sc.StartDate ?? sc.CreatedTime) <= endDate)
                .ToListAsync();

            foreach (var stock in stockChecks)
            {
                var stockDate = stock.CheckTime ?? stock.EndDate ?? stock.StartDate ?? stock.CreatedTime;
                events.Add(new CalendarEventResponse
                {
                    Id = stock.Id,
                    Title = $"Kiểm kê - {stock.Code}",
                    Date = stockDate,
                    Type = "stockcheck",
                    Color = "yellow",
                    Status = stock.Status,
                    WarehouseName = stock.Warehouse?.Name,
                    Code = stock.Code,
                    DetailUrl = $"/kiem-ke/chi-tiet/{stock.Id}"
                });
            }

            return events.OrderBy(e => e.Date).ToList();
        }
    }
}
