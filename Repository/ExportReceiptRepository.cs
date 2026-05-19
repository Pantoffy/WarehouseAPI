using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
using WarehouseAPI.DTOs.ExportReceiptDTOs;
using WarehouseAPI.Models;

namespace WarehouseAPI.Repository
{
    public interface IExportReceiptRepository
    {
        Task<List<ExportReceiptResponse>> GetAllExportReceiptAsync();
        Task<ExportReceiptResponse?> GetExportReceiptByIdAsync(int id);
        Task<ExportReceiptResponse> AddExportReceiptAsync(CreateExportReceiptRequest exportReceipt);
        Task<bool> UpdateExportReceiptByIdAsync(int id, UpdateExportReceiptRequest exportReceipt);
        Task<bool> DeleteExportReceiptByIdAsync(int id);
    }

    public class ExportReceiptRepository(AppDbContext context) : IExportReceiptRepository
    {
        public async Task<ExportReceiptResponse> AddExportReceiptAsync(CreateExportReceiptRequest exportReceipt)
        {
            // Validate WarehouseId exists
            var warehouseExists = await context.Warehouse.AnyAsync(w => w.Id == exportReceipt.WarehouseId);
            if (!warehouseExists)
                throw new InvalidOperationException($"Kho hàng có ID {exportReceipt.WarehouseId} không tồn tại.");

            // Check if ReceiptNumber already exists
            var receiptExists = await context.ExportReceipt.AnyAsync(er => er.ReceiptNumber == exportReceipt.ReceiptNumber);
            if (receiptExists)
                throw new InvalidOperationException($"Số phiếu xuất '{exportReceipt.ReceiptNumber}' đã tồn tại.");

            // Validate ExportReceiptDetails materials exist and check stock
            if (exportReceipt.ExportReceiptDetails?.Any() == true)
            {
                var duplicateMaterialIds = exportReceipt.ExportReceiptDetails
                    .GroupBy(d => d.MaterialId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateMaterialIds.Any())
                    throw new InvalidOperationException("Chi tiết phiếu xuất không được trùng vật liệu.");

                var materialIds = exportReceipt.ExportReceiptDetails.Select(d => d.MaterialId).Distinct();
                var existingMaterials = await context.Materials
                    .Where(m => materialIds.Contains(m.Id))
                    .Select(m => m.Id)
                    .ToListAsync();

                foreach (var materialId in materialIds)
                {
                    if (!existingMaterials.Contains(materialId))
                        throw new InvalidOperationException($"Vật liệu có ID {materialId} không tồn tại.");
                }

                // Check stock before export
                foreach (var detail in exportReceipt.ExportReceiptDetails)
                {
                    var inventory = await context.Inventory
                        .FirstOrDefaultAsync(i => i.WarehouseId == exportReceipt.WarehouseId && i.MaterialId == detail.MaterialId);

                    if (inventory is null || inventory.Quantity < detail.Quantity)
                        throw new InvalidOperationException(
                            $"Không đủ hàng tồn kho cho vật liệu ID {detail.MaterialId}. Tồn kho: {inventory?.Quantity ?? 0}, Yêu cầu: {detail.Quantity}");
                }
            }

            var normalizedStatus = NormalizeExportStatus(exportReceipt.Status);

            var newExportReceipt = new ExportReceipt
            {
                Code = exportReceipt.Code,
                ReceiptNumber = exportReceipt.ReceiptNumber,
                ExportDate = exportReceipt.ExportDate,
                WarehouseId = exportReceipt.WarehouseId,
                ReceiverName = exportReceipt.ReceiverName,
                Reason = exportReceipt.Reason,
                DocumentNo = exportReceipt.DocumentNo,
                TotalAmount = 0, // Will be calculated from details
                Status = normalizedStatus,
                CreatedBy = exportReceipt.CreatedBy,
                ApprovedBy = exportReceipt.ApprovedBy,
                ApprovedAt = exportReceipt.ApprovedAt,
                Note = exportReceipt.Note,
                CreatedAt = exportReceipt.CreatedAt
            };

            context.ExportReceipt.Add(newExportReceipt);
            await context.SaveChangesAsync();

            // Add ExportReceiptDetails with auto-calculated amount
            decimal totalAmount = 0;
            if (exportReceipt.ExportReceiptDetails?.Any() == true)
            {
                var details = exportReceipt.ExportReceiptDetails.Select(d => new ExportReceiptDetail
                {
                    ExportReceiptId = newExportReceipt.Id,
                    MaterialId = d.MaterialId,
                    UnitId = d.UnitId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Amount = d.Quantity * (d.UnitPrice ?? 0), // Auto-calculate
                    Note = d.Note
                }).ToList();

                totalAmount = details.Sum(d => d.Amount ?? 0);
                context.ExportReceiptDetail.AddRange(details);
            }

            // Update total amount
            newExportReceipt.TotalAmount = totalAmount;
            await context.SaveChangesAsync();

            // Reload full data to return
            var result = await context.ExportReceipt
                .Include(er => er.Warehouse)
                .Include(er => er.ExportReceiptDetails!)
                    .ThenInclude(erd => erd.Material)
                .Where(er => er.Id == newExportReceipt.Id)
                .Select(er => new ExportReceiptResponse
                {
                    Id = er.Id,
                    Code = er.Code,
                    ReceiptNumber = er.ReceiptNumber,
                    ExportDate = er.ExportDate,
                    WarehouseId = er.WarehouseId,
                    ReceiverName = er.ReceiverName,
                    Reason = er.Reason,
                    DocumentNo = er.DocumentNo,
                    TotalAmount = er.TotalAmount,
                    Status = er.Status,
                    CreatedBy = er.CreatedBy,
                    ApprovedBy = er.ApprovedBy,
                    ApprovedAt = er.ApprovedAt,
                    Note = er.Note,
                    CreatedAt = er.CreatedAt,
                    Warehouse = er.Warehouse,
                    ExportReceiptDetails = er.ExportReceiptDetails!.Select(d => new ExportReceiptDetailResponse
                    {
                        Id = d.Id,
                        ExportReceiptId = d.ExportReceiptId,
                        MaterialId = d.MaterialId,
                        UnitId = d.UnitId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Amount = d.Amount,
                        Note = d.Note
                    }).ToList()
                }).FirstOrDefaultAsync();

            return result!;
        }

        public async Task<bool> DeleteExportReceiptByIdAsync(int id)
        {
            var exportReceipt = await context.ExportReceipt
                .Include(er => er.ExportReceiptDetails)
                .FirstOrDefaultAsync(er => er.Id == id);

            if (exportReceipt is null)
                return false;

            // Delete all related ExportReceiptDetails first
            if (exportReceipt.ExportReceiptDetails?.Any() == true)
            {
                context.ExportReceiptDetail.RemoveRange(exportReceipt.ExportReceiptDetails);
            }

            // Then delete the ExportReceipt
            context.ExportReceipt.Remove(exportReceipt);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ExportReceiptResponse>> GetAllExportReceiptAsync()
            => await context.ExportReceipt
                .Include(er => er.Warehouse)
                .Include(er => er.ExportReceiptDetails!)
                    .ThenInclude(erd => erd.Material)
                .Select(er => new ExportReceiptResponse
                {
                    Id = er.Id,
                    Code = er.Code,
                    ReceiptNumber = er.ReceiptNumber,
                    ExportDate = er.ExportDate,
                    WarehouseId = er.WarehouseId,
                    ReceiverName = er.ReceiverName,
                    Reason = er.Reason,
                    DocumentNo = er.DocumentNo,
                    TotalAmount = er.TotalAmount,
                    Status = er.Status,
                    CreatedBy = er.CreatedBy,
                    ApprovedBy = er.ApprovedBy,
                    ApprovedAt = er.ApprovedAt,
                    Note = er.Note,
                    CreatedAt = er.CreatedAt,
                    Warehouse = er.Warehouse,
                    ExportReceiptDetails = er.ExportReceiptDetails!.Select(d => new ExportReceiptDetailResponse
                    {
                        Id = d.Id,
                        ExportReceiptId = d.ExportReceiptId,
                        MaterialId = d.MaterialId,
                        UnitId = d.UnitId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Amount = d.Amount,
                        Note = d.Note
                    }).ToList()
                }).ToListAsync();

        public async Task<ExportReceiptResponse?> GetExportReceiptByIdAsync(int id)
        {
            var result = await context.ExportReceipt
                .Include(er => er.Warehouse)
                .Include(er => er.ExportReceiptDetails!)
                    .ThenInclude(erd => erd.Material)
                .Where(er => er.Id == id)
                .Select(er => new ExportReceiptResponse
                {
                    Id = er.Id,
                    Code = er.Code,
                    ReceiptNumber = er.ReceiptNumber,
                    ExportDate = er.ExportDate,
                    WarehouseId = er.WarehouseId,
                    ReceiverName = er.ReceiverName,
                    Reason = er.Reason,
                    DocumentNo = er.DocumentNo,
                    TotalAmount = er.TotalAmount,
                    Status = er.Status,
                    CreatedBy = er.CreatedBy,
                    ApprovedBy = er.ApprovedBy,
                    ApprovedAt = er.ApprovedAt,
                    Note = er.Note,
                    CreatedAt = er.CreatedAt,
                    Warehouse = er.Warehouse,
                    ExportReceiptDetails = er.ExportReceiptDetails!.Select(d => new ExportReceiptDetailResponse
                    {
                        Id = d.Id,
                        ExportReceiptId = d.ExportReceiptId,
                        MaterialId = d.MaterialId,
                        UnitId = d.UnitId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Amount = d.Amount,
                        Note = d.Note
                    }).ToList()
                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> UpdateExportReceiptByIdAsync(int id, UpdateExportReceiptRequest exportReceipt)
        {
            var existingExportReceipt = await context.ExportReceipt
                .Include(er => er.ExportReceiptDetails)
                .FirstOrDefaultAsync(er => er.Id == id);
            
            if (existingExportReceipt is null)
                return false;

            // Validate WarehouseId exists
            var warehouseExists = await context.Warehouse.AnyAsync(w => w.Id == exportReceipt.WarehouseId);
            if (!warehouseExists)
                throw new InvalidOperationException($"Kho hàng có ID {exportReceipt.WarehouseId} không tồn tại.");

            // Check if ReceiptNumber already exists (excluding current record)
            var receiptExists = await context.ExportReceipt.AnyAsync(er => er.Id != id && er.ReceiptNumber == exportReceipt.ReceiptNumber);
            if (receiptExists)
                throw new InvalidOperationException($"Số phiếu xuất '{exportReceipt.ReceiptNumber}' đã tồn tại.");

            // Validate materials exist if ExportReceiptDetails provided
            if (exportReceipt.ExportReceiptDetails?.Any() == true)
            {
                var duplicateMaterialIds = exportReceipt.ExportReceiptDetails
                    .GroupBy(d => d.MaterialId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateMaterialIds.Any())
                    throw new InvalidOperationException("Chi tiết phiếu xuất không được trùng vật liệu.");

                var materialIds = exportReceipt.ExportReceiptDetails.Select(d => d.MaterialId).Distinct();
                var existingMaterials = await context.Materials
                    .Where(m => materialIds.Contains(m.Id))
                    .Select(m => m.Id)
                    .ToListAsync();

                foreach (var materialId in materialIds)
                {
                    if (!existingMaterials.Contains(materialId))
                        throw new InvalidOperationException($"Vật liệu có ID {materialId} không tồn tại.");
                }
            }

            existingExportReceipt.Code = exportReceipt.Code;
            existingExportReceipt.ReceiptNumber = exportReceipt.ReceiptNumber;
            existingExportReceipt.ExportDate = exportReceipt.ExportDate;
            existingExportReceipt.WarehouseId = exportReceipt.WarehouseId;
            existingExportReceipt.ReceiverName = exportReceipt.ReceiverName;
            existingExportReceipt.Reason = exportReceipt.Reason;
            existingExportReceipt.DocumentNo = exportReceipt.DocumentNo;
            existingExportReceipt.Status = NormalizeExportStatus(exportReceipt.Status);
            if (!string.IsNullOrEmpty(exportReceipt.CreatedBy))
                existingExportReceipt.CreatedBy = exportReceipt.CreatedBy;
            if (!string.IsNullOrEmpty(exportReceipt.ApprovedBy))
                existingExportReceipt.ApprovedBy = exportReceipt.ApprovedBy;
            if (exportReceipt.ApprovedAt.HasValue)
                existingExportReceipt.ApprovedAt = exportReceipt.ApprovedAt;
            existingExportReceipt.Note = exportReceipt.Note;
            if (exportReceipt.CreatedAt.HasValue && exportReceipt.CreatedAt.Value > DateTime.MinValue)
            {
                existingExportReceipt.CreatedAt = exportReceipt.CreatedAt.Value;
            }

            // Update ExportReceiptDetails if provided
            if (exportReceipt.ExportReceiptDetails != null)
            {
                // Remove old details
                if (existingExportReceipt.ExportReceiptDetails?.Any() == true)
                {
                    context.ExportReceiptDetail.RemoveRange(existingExportReceipt.ExportReceiptDetails);
                }

                // Add new details
                decimal totalAmount = 0;
                var newDetails = exportReceipt.ExportReceiptDetails.Select(d => new ExportReceiptDetail
                {
                    ExportReceiptId = id,
                    MaterialId = d.MaterialId,
                    UnitId = d.UnitId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Amount = d.Quantity * (d.UnitPrice ?? 0),
                    Note = d.Note
                }).ToList();

                totalAmount = newDetails.Sum(d => d.Amount ?? 0);
                context.ExportReceiptDetail.AddRange(newDetails);
                existingExportReceipt.TotalAmount = totalAmount;
            }
            else
            {
                existingExportReceipt.TotalAmount = exportReceipt.TotalAmount;
            }

            await context.SaveChangesAsync();
            return true;
        }

        private static string NormalizeExportStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new InvalidOperationException("Trạng thái phiếu xuất là bắt buộc.");

            var value = status.Trim();

            if (value.Equals("Đã xác nhận", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Approved", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Đã duyệt", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Hoàn thành", StringComparison.OrdinalIgnoreCase))
            {
                return "Đã xác nhận";
            }

            if (value.Equals("Đã hủy", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Cancel", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Hủy", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Bị hủy", StringComparison.OrdinalIgnoreCase))
            {
                return "Đã hủy";
            }

            if (value.Equals("Chờ xác nhận", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Pending", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Chờ duyệt", StringComparison.OrdinalIgnoreCase))
            {
                return "Chờ xác nhận";
            }

            if (value.Equals("Đang soạn thảo", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Draft", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Nháp", StringComparison.OrdinalIgnoreCase))
            {
                return "Đang soạn thảo";
            }

            throw new InvalidOperationException($"Trạng thái phiếu xuất không hợp lệ: {status}");
        }
    }
}
