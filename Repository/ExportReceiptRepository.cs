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
                throw new InvalidOperationException($"Warehouse with ID {exportReceipt.WarehouseId} does not exist.");

            // Check if ReceiptNumber already exists
            var receiptExists = await context.ExportReceipt.AnyAsync(er => er.ReceiptNumber == exportReceipt.ReceiptNumber);
            if (receiptExists)
                throw new InvalidOperationException($"Receipt number '{exportReceipt.ReceiptNumber}' already exists.");

            var newExportReceipt = new ExportReceipt
            {
                Code = exportReceipt.Code,
                ReceiptNumber = exportReceipt.ReceiptNumber,
                ExportDate = exportReceipt.ExportDate,
                WarehouseId = exportReceipt.WarehouseId,
                ReceiverName = exportReceipt.ReceiverName,
                Reason = exportReceipt.Reason,
                DocumentNo = exportReceipt.DocumentNo,
                TotalAmount = exportReceipt.TotalAmount,
                Status = exportReceipt.Status,
                CreatedBy = exportReceipt.CreatedBy,
                ApprovedBy = exportReceipt.ApprovedBy,
                ApprovedAt = exportReceipt.ApprovedAt,
                Note = exportReceipt.Note,
                CreatedAt = exportReceipt.CreatedAt
            };

            context.ExportReceipt.Add(newExportReceipt);
            await context.SaveChangesAsync();

            return new ExportReceiptResponse
            {
                Id = newExportReceipt.Id,
                Code = newExportReceipt.Code,
                ReceiptNumber = newExportReceipt.ReceiptNumber,
                ExportDate = newExportReceipt.ExportDate,
                WarehouseId = newExportReceipt.WarehouseId,
                ReceiverName = newExportReceipt.ReceiverName,
                Reason = newExportReceipt.Reason,
                DocumentNo = newExportReceipt.DocumentNo,
                TotalAmount = newExportReceipt.TotalAmount,
                Status = newExportReceipt.Status,
                CreatedBy = newExportReceipt.CreatedBy,
                ApprovedBy = newExportReceipt.ApprovedBy,
                ApprovedAt = newExportReceipt.ApprovedAt,
                Note = newExportReceipt.Note,
                CreatedAt = newExportReceipt.CreatedAt
            };
        }

        public async Task<bool> DeleteExportReceiptByIdAsync(int id)
        {
            var exportReceipt = await context.ExportReceipt.FindAsync(id);
            if (exportReceipt is null)
                return false;

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
                    ExportReceiptDetails = er.ExportReceiptDetails
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
                    ExportReceiptDetails = er.ExportReceiptDetails
                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> UpdateExportReceiptByIdAsync(int id, UpdateExportReceiptRequest exportReceipt)
        {
            var existingExportReceipt = await context.ExportReceipt.FindAsync(id);
            if (existingExportReceipt is null)
                return false;

            // Validate WarehouseId exists
            var warehouseExists = await context.Warehouse.AnyAsync(w => w.Id == exportReceipt.WarehouseId);
            if (!warehouseExists)
                throw new InvalidOperationException($"Warehouse with ID {exportReceipt.WarehouseId} does not exist.");

            // Check if ReceiptNumber already exists (excluding current record)
            var receiptExists = await context.ExportReceipt.AnyAsync(er => er.Id != id && er.ReceiptNumber == exportReceipt.ReceiptNumber);
            if (receiptExists)
                throw new InvalidOperationException($"Receipt number '{exportReceipt.ReceiptNumber}' already exists.");

            existingExportReceipt.Code = exportReceipt.Code;
            existingExportReceipt.ReceiptNumber = exportReceipt.ReceiptNumber;
            existingExportReceipt.ExportDate = exportReceipt.ExportDate;
            existingExportReceipt.WarehouseId = exportReceipt.WarehouseId;
            existingExportReceipt.ReceiverName = exportReceipt.ReceiverName;
            existingExportReceipt.Reason = exportReceipt.Reason;
            existingExportReceipt.DocumentNo = exportReceipt.DocumentNo;
            existingExportReceipt.TotalAmount = exportReceipt.TotalAmount;
            existingExportReceipt.Status = exportReceipt.Status;
            existingExportReceipt.CreatedBy = exportReceipt.CreatedBy;
            existingExportReceipt.ApprovedBy = exportReceipt.ApprovedBy;
            existingExportReceipt.ApprovedAt = exportReceipt.ApprovedAt;
            existingExportReceipt.Note = exportReceipt.Note;
            existingExportReceipt.CreatedAt = exportReceipt.CreatedAt;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
