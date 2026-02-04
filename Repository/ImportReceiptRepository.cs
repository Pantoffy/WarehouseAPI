using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
using WarehouseAPI.DTOs.ImportReceiptDTOs;
using WarehouseAPI.Models;

namespace WarehouseAPI.Repository
{
    public interface IImportReceiptRepository
    {
        Task<List<ImportReceiptResponse>> GetAllImportReceiptAsync();
        Task<ImportReceiptResponse?> GetImportReceiptByIdAsync(int id);
        Task<ImportReceiptResponse> AddImportReceiptAsync(CreateImportReceiptRequest importReceipt);
        Task<bool> UpdateImportReceiptByIdAsync(int id, UpdateImportReceiptRequest importReceipt);
        Task<bool> DeleteImportReceiptByIdAsync(int id);
    }

    public class ImportReceiptRepository(AppDbContext context) : IImportReceiptRepository
    {
        public async Task<ImportReceiptResponse> AddImportReceiptAsync(CreateImportReceiptRequest importReceipt)
        {
            // Validate SupplierId exists
            var supplierExists = await context.Suppliers.AnyAsync(s => s.Id == importReceipt.SupplierId);
            if (!supplierExists)
                throw new InvalidOperationException($"Supplier with ID {importReceipt.SupplierId} does not exist.");

            // Validate WarehouseId exists
            var warehouseExists = await context.Warehouse.AnyAsync(w => w.Id == importReceipt.WarehouseId);
            if (!warehouseExists)
                throw new InvalidOperationException($"Warehouse with ID {importReceipt.WarehouseId} does not exist.");

            // Check if ReceiptNumber already exists
            var receiptExists = await context.ImportReceipt.AnyAsync(ir => ir.ReceiptNumber == importReceipt.ReceiptNumber);
            if (receiptExists)
                throw new InvalidOperationException($"Receipt number '{importReceipt.ReceiptNumber}' already exists.");

            var newImportReceipt = new ImportReceipt
            {
                Code = importReceipt.Code,
                ReceiptNumber = importReceipt.ReceiptNumber,
                ImportTime = importReceipt.ImportTime,
                SupplierId = importReceipt.SupplierId,
                WarehouseId = importReceipt.WarehouseId,
                SupplierInvoiceNo = importReceipt.SupplierInvoiceNo,
                DocumentNo = importReceipt.DocumentNo,
                TotalAmount = importReceipt.TotalAmount,
                Status = importReceipt.Status,
                CreatedBy = importReceipt.CreatedBy,
                ApprovedBy = importReceipt.ApprovedBy,
                ApprovedAt = importReceipt.ApprovedAt,
                Note = importReceipt.Note,
                CreatedAt = importReceipt.CreatedAt
            };

            context.ImportReceipt.Add(newImportReceipt);
            await context.SaveChangesAsync();

            return new ImportReceiptResponse
            {
                Id = newImportReceipt.Id,
                Code = newImportReceipt.Code,
                ReceiptNumber = newImportReceipt.ReceiptNumber,
                ImportTime = newImportReceipt.ImportTime,
                SupplierId = newImportReceipt.SupplierId,
                WarehouseId = newImportReceipt.WarehouseId,
                SupplierInvoiceNo = newImportReceipt.SupplierInvoiceNo,
                DocumentNo = newImportReceipt.DocumentNo,
                TotalAmount = newImportReceipt.TotalAmount,
                Status = newImportReceipt.Status,
                CreatedBy = newImportReceipt.CreatedBy,
                ApprovedBy = newImportReceipt.ApprovedBy,
                ApprovedAt = newImportReceipt.ApprovedAt,
                Note = newImportReceipt.Note,
                CreatedAt = newImportReceipt.CreatedAt
            };
        }

        public async Task<bool> DeleteImportReceiptByIdAsync(int id)
        {
            var importReceipt = await context.ImportReceipt.FindAsync(id);
            if (importReceipt is null)
                return false;

            context.ImportReceipt.Remove(importReceipt);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ImportReceiptResponse>> GetAllImportReceiptAsync()
            => await context.ImportReceipt
                .Include(ir => ir.Supplier)
                .Include(ir => ir.Warehouse)
                .Include(ir => ir.ImportReceiptDetails!)
                    .ThenInclude(ird => ird.Material)
                .Select(ir => new ImportReceiptResponse
                {
                    Id = ir.Id,
                    Code = ir.Code,
                    ReceiptNumber = ir.ReceiptNumber,
                    ImportTime = ir.ImportTime,
                    SupplierId = ir.SupplierId,
                    WarehouseId = ir.WarehouseId,
                    SupplierInvoiceNo = ir.SupplierInvoiceNo,
                    DocumentNo = ir.DocumentNo,
                    TotalAmount = ir.TotalAmount,
                    Status = ir.Status,
                    CreatedBy = ir.CreatedBy,
                    ApprovedBy = ir.ApprovedBy,
                    ApprovedAt = ir.ApprovedAt,
                    Note = ir.Note,
                    CreatedAt = ir.CreatedAt,
                    Supplier = ir.Supplier,
                    Warehouse = ir.Warehouse,
                    ImportReceiptDetails = ir.ImportReceiptDetails
                }).ToListAsync();

        public async Task<ImportReceiptResponse?> GetImportReceiptByIdAsync(int id)
        {
            var result = await context.ImportReceipt
                .Include(ir => ir.Supplier)
                .Include(ir => ir.Warehouse)
                .Include(ir => ir.ImportReceiptDetails!)
                    .ThenInclude(ird => ird.Material)
                .Where(ir => ir.Id == id)
                .Select(ir => new ImportReceiptResponse
                {
                    Id = ir.Id,
                    Code = ir.Code,
                    ReceiptNumber = ir.ReceiptNumber,
                    ImportTime = ir.ImportTime,
                    SupplierId = ir.SupplierId,
                    WarehouseId = ir.WarehouseId,
                    SupplierInvoiceNo = ir.SupplierInvoiceNo,
                    DocumentNo = ir.DocumentNo,
                    TotalAmount = ir.TotalAmount,
                    Status = ir.Status,
                    CreatedBy = ir.CreatedBy,
                    ApprovedBy = ir.ApprovedBy,
                    ApprovedAt = ir.ApprovedAt,
                    Note = ir.Note,
                    CreatedAt = ir.CreatedAt,
                    Supplier = ir.Supplier,
                    Warehouse = ir.Warehouse,
                    ImportReceiptDetails = ir.ImportReceiptDetails
                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> UpdateImportReceiptByIdAsync(int id, UpdateImportReceiptRequest importReceipt)
        {
            var existingImportReceipt = await context.ImportReceipt.FindAsync(id);
            if (existingImportReceipt is null)
                return false;

            // Validate SupplierId exists
            var supplierExists = await context.Suppliers.AnyAsync(s => s.Id == importReceipt.SupplierId);
            if (!supplierExists)
                throw new InvalidOperationException($"Supplier with ID {importReceipt.SupplierId} does not exist.");

            // Validate WarehouseId exists
            var warehouseExists = await context.Warehouse.AnyAsync(w => w.Id == importReceipt.WarehouseId);
            if (!warehouseExists)
                throw new InvalidOperationException($"Warehouse with ID {importReceipt.WarehouseId} does not exist.");

            // Check if ReceiptNumber already exists (excluding current record)
            var receiptExists = await context.ImportReceipt.AnyAsync(ir => ir.Id != id && ir.ReceiptNumber == importReceipt.ReceiptNumber);
            if (receiptExists)
                throw new InvalidOperationException($"Receipt number '{importReceipt.ReceiptNumber}' already exists.");

            existingImportReceipt.Code = importReceipt.Code;
            existingImportReceipt.ReceiptNumber = importReceipt.ReceiptNumber;
            existingImportReceipt.ImportTime = importReceipt.ImportTime;
            existingImportReceipt.SupplierId = importReceipt.SupplierId;
            existingImportReceipt.WarehouseId = importReceipt.WarehouseId;
            existingImportReceipt.SupplierInvoiceNo = importReceipt.SupplierInvoiceNo;
            existingImportReceipt.DocumentNo = importReceipt.DocumentNo;
            existingImportReceipt.TotalAmount = importReceipt.TotalAmount;
            existingImportReceipt.Status = importReceipt.Status;
            existingImportReceipt.CreatedBy = importReceipt.CreatedBy;
            existingImportReceipt.ApprovedBy = importReceipt.ApprovedBy;
            existingImportReceipt.ApprovedAt = importReceipt.ApprovedAt;
            existingImportReceipt.Note = importReceipt.Note;
            existingImportReceipt.CreatedAt = importReceipt.CreatedAt;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
