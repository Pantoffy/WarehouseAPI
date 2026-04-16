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
                Status = exportReceipt.Status,
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
            existingExportReceipt.Status = exportReceipt.Status;
            existingExportReceipt.CreatedBy = exportReceipt.CreatedBy;
            existingExportReceipt.ApprovedBy = exportReceipt.ApprovedBy;
            existingExportReceipt.ApprovedAt = exportReceipt.ApprovedAt;
            existingExportReceipt.Note = exportReceipt.Note;
            existingExportReceipt.CreatedAt = exportReceipt.CreatedAt;

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
    }
}
