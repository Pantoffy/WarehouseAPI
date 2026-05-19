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
                throw new InvalidOperationException($"Nhà cung cấp có ID {importReceipt.SupplierId} không tồn tại.");

            // Validate WarehouseId exists
            var warehouseExists = await context.Warehouse.AnyAsync(w => w.Id == importReceipt.WarehouseId);
            if (!warehouseExists)
                throw new InvalidOperationException($"Kho hàng có ID {importReceipt.WarehouseId} không tồn tại.");

            // Check if ReceiptNumber already exists
            var receiptExists = await context.ImportReceipt.AnyAsync(ir => ir.ReceiptNumber == importReceipt.ReceiptNumber);
            if (receiptExists)
                throw new InvalidOperationException($"Số phiếu nhập '{importReceipt.ReceiptNumber}' đã tồn tại.");

            // Validate ImportReceiptDetails materials exist
            if (importReceipt.ImportReceiptDetails?.Any() == true)
            {
                var duplicateMaterialIds = importReceipt.ImportReceiptDetails
                    .GroupBy(d => d.MaterialId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateMaterialIds.Any())
                    throw new InvalidOperationException("Chi tiết phiếu nhập không được trùng vật liệu.");

                var materialIds = importReceipt.ImportReceiptDetails.Select(d => d.MaterialId).Distinct();
                var existingMaterials = await context.Materials.Where(m => materialIds.Contains(m.Id)).Select(m => m.Id).ToListAsync();

                foreach (var materialId in materialIds)
                {
                    if (!existingMaterials.Contains(materialId))
                        throw new InvalidOperationException($"Vật liệu có ID {materialId} không tồn tại.");
                }
            }

            var normalizedStatus = NormalizeImportStatus(importReceipt.Status);

            var newImportReceipt = new ImportReceipt
            {
                Code = importReceipt.Code,
                ReceiptNumber = importReceipt.ReceiptNumber,
                ImportTime = importReceipt.ImportTime,
                SupplierId = importReceipt.SupplierId,
                WarehouseId = importReceipt.WarehouseId,
                SupplierInvoiceNo = importReceipt.SupplierInvoiceNo,
                DocumentNo = importReceipt.DocumentNo,
                TotalAmount = 0, // Will be calculated from details
                Status = normalizedStatus,
                CreatedBy = importReceipt.CreatedBy,
                ApprovedBy = importReceipt.ApprovedBy,
                ApprovedAt = importReceipt.ApprovedAt,
                Note = importReceipt.Note,
                CreatedAt = importReceipt.CreatedAt
            };

            context.ImportReceipt.Add(newImportReceipt);
            await context.SaveChangesAsync();

            // Add ImportReceiptDetails with auto-calculated amount
            decimal totalAmount = 0;
            if (importReceipt.ImportReceiptDetails?.Any() == true)
            {
                var details = importReceipt.ImportReceiptDetails.Select(d => new ImportReceiptDetail
                {
                    ImportReceiptId = newImportReceipt.Id,
                    MaterialId = d.MaterialId,
                    UnitId = d.UnitId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Amount = d.Quantity * d.UnitPrice, // Auto-calculate
                    Note = d.Note
                }).ToList();

                totalAmount = details.Sum(d => d.Amount ?? 0);
                context.ImportReceiptDetail.AddRange(details);
                await context.SaveChangesAsync();
            }

            // Update total amount
            newImportReceipt.TotalAmount = totalAmount;
            await context.SaveChangesAsync();

            // Reload full data to return
            var result = await context.ImportReceipt
                .Include(ir => ir.Supplier)
                .Include(ir => ir.Warehouse)
                .Include(ir => ir.ImportReceiptDetails!)
                    .ThenInclude(ird => ird.Material)
                .Where(ir => ir.Id == newImportReceipt.Id)
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
                    ImportReceiptDetails = ir.ImportReceiptDetails!.Select(d => new ImportReceiptDetailResponse
                    {
                        Id = d.Id,
                        ImportReceiptId = d.ImportReceiptId,
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

        public async Task<bool> DeleteImportReceiptByIdAsync(int id)
        {
            var importReceipt = await context.ImportReceipt
                .Include(ir => ir.ImportReceiptDetails)
                .FirstOrDefaultAsync(ir => ir.Id == id);

            if (importReceipt is null)
                return false;

            // Delete all related ImportReceiptDetails first
            if (importReceipt.ImportReceiptDetails?.Any() == true)
            {
                context.ImportReceiptDetail.RemoveRange(importReceipt.ImportReceiptDetails);
            }

            // Then delete the ImportReceipt
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
                    ImportReceiptDetails = ir.ImportReceiptDetails!.Select(d => new ImportReceiptDetailResponse
                    {
                        Id = d.Id,
                        ImportReceiptId = d.ImportReceiptId,
                        MaterialId = d.MaterialId,
                        UnitId = d.UnitId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Amount = d.Amount,
                        Note = d.Note
                    }).ToList()
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
                    ImportReceiptDetails = ir.ImportReceiptDetails!.Select(d => new ImportReceiptDetailResponse
                    {
                        Id = d.Id,
                        ImportReceiptId = d.ImportReceiptId,
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

        public async Task<bool> UpdateImportReceiptByIdAsync(int id, UpdateImportReceiptRequest importReceipt)
        {
            var existingImportReceipt = await context.ImportReceipt
                .Include(ir => ir.ImportReceiptDetails)
                .FirstOrDefaultAsync(ir => ir.Id == id);

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

            // Validate ImportReceiptDetails materials exist
            if (importReceipt.ImportReceiptDetails?.Any() == true)
            {
                var duplicateMaterialIds = importReceipt.ImportReceiptDetails
                    .GroupBy(d => d.MaterialId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateMaterialIds.Any())
                    throw new InvalidOperationException("Chi tiết phiếu nhập không được trùng vật liệu.");

                var materialIds = importReceipt.ImportReceiptDetails.Select(d => d.MaterialId).Distinct();
                var existingMaterials = await context.Materials.Where(m => materialIds.Contains(m.Id)).Select(m => m.Id).ToListAsync();

                foreach (var materialId in materialIds)
                {
                    if (!existingMaterials.Contains(materialId))
                        throw new InvalidOperationException($"Material with ID {materialId} does not exist.");
                }
            }

            existingImportReceipt.Code = importReceipt.Code;
            existingImportReceipt.ReceiptNumber = importReceipt.ReceiptNumber;
            existingImportReceipt.ImportTime = importReceipt.ImportTime;
            existingImportReceipt.SupplierId = importReceipt.SupplierId;
            existingImportReceipt.WarehouseId = importReceipt.WarehouseId;
            existingImportReceipt.SupplierInvoiceNo = importReceipt.SupplierInvoiceNo;
            existingImportReceipt.DocumentNo = importReceipt.DocumentNo;
            existingImportReceipt.TotalAmount = importReceipt.TotalAmount;
            existingImportReceipt.Status = NormalizeImportStatus(importReceipt.Status);
            if (!string.IsNullOrEmpty(importReceipt.CreatedBy))
                existingImportReceipt.CreatedBy = importReceipt.CreatedBy;
            if (!string.IsNullOrEmpty(importReceipt.ApprovedBy))
                existingImportReceipt.ApprovedBy = importReceipt.ApprovedBy;
            if (importReceipt.ApprovedAt.HasValue)
                existingImportReceipt.ApprovedAt = importReceipt.ApprovedAt;
            existingImportReceipt.Note = importReceipt.Note;
            if (importReceipt.CreatedAt.HasValue && importReceipt.CreatedAt.Value > DateTime.MinValue)
            {
                existingImportReceipt.CreatedAt = importReceipt.CreatedAt.Value;
            }

            // Remove old ImportReceiptDetails
            if (existingImportReceipt.ImportReceiptDetails?.Any() == true)
            {
                context.ImportReceiptDetail.RemoveRange(existingImportReceipt.ImportReceiptDetails);
            }

            // Add new ImportReceiptDetails
            if (importReceipt.ImportReceiptDetails?.Any() == true)
            {
                var newDetails = importReceipt.ImportReceiptDetails.Select(d => new ImportReceiptDetail
                {
                    ImportReceiptId = id,
                    MaterialId = d.MaterialId,
                    UnitId = d.UnitId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Amount = d.Amount,
                    Note = d.Note
                }).ToList();

                context.ImportReceiptDetail.AddRange(newDetails);
            }

            await context.SaveChangesAsync();
            return true;
        }

        private static string NormalizeImportStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new InvalidOperationException("Trạng thái phiếu nhập là bắt buộc.");

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

            throw new InvalidOperationException($"Trạng thái phiếu nhập không hợp lệ: {status}");
        }
    }
}
