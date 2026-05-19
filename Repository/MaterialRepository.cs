using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
using WarehouseAPI.DTOs;
using WarehouseAPI.DTOs.MaterialDTOs;
using WarehouseAPI.Models;

namespace WarehouseAPI.Repository
{
    public interface IMaterialRepository
    {
        Task<List<MaterialResponse>> GetAllMaterialAsync();
        Task<MaterialResponse?> GetMaterialByIdAsync(int id);
        Task<MaterialResponse> AddMaterialAsync(CreateMaterialRequest material);
        Task<bool> UpdateMaterialByIdAsync(int id, UpdateMaterialRequest material);
        Task<bool> DeleteMaterialByIdAsync(int id);
    }

    public class MaterialRepository(AppDbContext context) : IMaterialRepository
    {
        public async Task<MaterialResponse> AddMaterialAsync(CreateMaterialRequest material)
        {
            if (!CategoryMapping.IsValidCategory(material.CategoryId))
                throw new InvalidOperationException("Loại vật tư không hợp lệ.");

            if (!UnitMapping.IsValidUnit(material.UnitId))
                throw new InvalidOperationException("Đơn vị tính không hợp lệ.");

            if (!CategoryUnitRule.IsValidUnit(material.CategoryId, material.UnitId))
                throw new InvalidOperationException("Đơn vị không hợp lệ với loại vật tư.");

            // Validate SupplierId exists (only when a supplier is specified)
            if (material.SupplierId > 0)
            {
                var supplierExists = await context.Suppliers.AnyAsync(s => s.Id == material.SupplierId);
                if (!supplierExists)
                    throw new InvalidOperationException($"Nhà cung cấp có ID {material.SupplierId} không tồn tại.");
            }

            var itemType = NormalizeItemType(material.ItemType);

            var newMaterial = new Material
            {
                Code = material.Code,
                Name = material.Name,
                CategoryId = material.CategoryId,
                UnitId = material.UnitId,
                SupplierId = material.SupplierId,
                ItemType = itemType,
                Note = material.Note,
                Status = material.Status,
                CreatedTime = material.CreatedTime
            };

            context.Materials.Add(newMaterial);
            await context.SaveChangesAsync();

            // Create default inventory rows with Quantity = 0 for all existing warehouses
            var warehouses = await context.Warehouse.Select(w => w.Id).ToListAsync();
            if (warehouses.Count > 0)
            {
                var defaultInventories = warehouses.Select(warehouseId => new Inventory
                {
                    WarehouseId = warehouseId,
                    MaterialId = newMaterial.Id,
                    Quantity = 0,
                    UpdatedDate = DateTime.Now
                });

                context.Inventory.AddRange(defaultInventories);
                await context.SaveChangesAsync();
            }

            return new MaterialResponse
            {
                Id = newMaterial.Id,
                Code = newMaterial.Code,
                Name = newMaterial.Name,
                CategoryId = newMaterial.CategoryId,
                UnitId = newMaterial.UnitId,
                SupplierId = newMaterial.SupplierId,
                ItemType = newMaterial.ItemType,
                Note = newMaterial.Note,
                Status = newMaterial.Status,
                CreatedTime = newMaterial.CreatedTime
            };
        }

        public async Task<bool> DeleteMaterialByIdAsync(int id)
        {
            var material = await context.Materials.FindAsync(id);
            if (material is null)
                return false;

            // Check and delete related ImportReceiptDetails
            var importReceiptDetails = await context.ImportReceiptDetail
                .Where(ird => ird.MaterialId == id)
                .ToListAsync();

            if (importReceiptDetails.Any())
            {
                context.ImportReceiptDetail.RemoveRange(importReceiptDetails);
            }

            context.Materials.Remove(material);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MaterialResponse>> GetAllMaterialAsync()
            => await context.Materials
                .Include(m => m.Supplier)
                .Select(m => new MaterialResponse
                {
                    Id = m.Id,
                    Code = m.Code,
                    Name = m.Name,
                    CategoryId = m.CategoryId,
                    UnitId = m.UnitId,
                    SupplierId = m.SupplierId,
                    ItemType = m.ItemType,
                    Note = m.Note,
                    Status = m.Status,
                    CreatedTime = m.CreatedTime,
                    Supplier = m.Supplier
                }).ToListAsync();

        public async Task<MaterialResponse?> GetMaterialByIdAsync(int id)
        {
            var result = await context.Materials
                .Include(m => m.Supplier)
                .Where(m => m.Id == id)
                .Select(m => new MaterialResponse
                {
                    Id = m.Id,
                    Code = m.Code,
                    Name = m.Name,
                    CategoryId = m.CategoryId,
                    UnitId = m.UnitId,
                    SupplierId = m.SupplierId,
                    ItemType = m.ItemType,
                    Note = m.Note,
                    Status = m.Status,
                    CreatedTime = m.CreatedTime,
                    Supplier = m.Supplier
                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> UpdateMaterialByIdAsync(int id, UpdateMaterialRequest material)
        {
            var existingMaterial = await context.Materials.FindAsync(id);
            if (existingMaterial is null)
                return false;

            if (!CategoryMapping.IsValidCategory(material.CategoryId))
                throw new InvalidOperationException("Loại vật tư không hợp lệ.");

            if (!UnitMapping.IsValidUnit(material.UnitId))
                throw new InvalidOperationException("Đơn vị tính không hợp lệ.");

            if (!CategoryUnitRule.IsValidUnit(material.CategoryId, material.UnitId))
                throw new InvalidOperationException("Đơn vị không hợp lệ với loại vật tư.");

            // Validate SupplierId exists (only when a supplier is specified)
            if (material.SupplierId > 0)
            {
                var supplierExists = await context.Suppliers.AnyAsync(s => s.Id == material.SupplierId);
                if (!supplierExists)
                    throw new InvalidOperationException($"Nhà cung cấp có ID {material.SupplierId} không tồn tại.");
            }

            existingMaterial.Code = material.Code;
            existingMaterial.Name = material.Name;
            existingMaterial.CategoryId = material.CategoryId;
            existingMaterial.UnitId = material.UnitId;
            existingMaterial.SupplierId = material.SupplierId;
            existingMaterial.ItemType = NormalizeItemType(material.ItemType);
            existingMaterial.Note = material.Note;
            existingMaterial.Status = material.Status;
            existingMaterial.CreatedTime = material.CreatedTime;

            await context.SaveChangesAsync();
            return true;
        }

        private static string NormalizeItemType(string? itemType)
        {
            if (string.IsNullOrWhiteSpace(itemType))
                return "Nguyên liệu";

            var value = itemType.Trim().ToLowerInvariant();

            // Accept both English (frontend) and Vietnamese values
            if (value is "material" or "nguyen lieu" or "nguyên liệu")
                return "Nguyên liệu";

            if (value is "asset" or "tai san" or "tài sản")
                return "Tài sản";

            if (value is "goods" or "hang hoa" or "hàng hóa" or "product" or "products")
                return "Hàng hóa";

            throw new InvalidOperationException($"Loại vật tư không hợp lệ: {itemType}");
        }
    }
}

