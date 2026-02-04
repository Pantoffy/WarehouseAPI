using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
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
            // Validate SupplierId exists
            var supplierExists = await context.Suppliers.AnyAsync(s => s.Id == material.SupplierId);
            if (!supplierExists)
                throw new InvalidOperationException($"Supplier with ID {material.SupplierId} does not exist.");

            var newMaterial = new Material
            {
                Code = material.Code,
                Name = material.Name,
                CategoryId = material.CategoryId,
                UnitId = material.UnitId,
                SupplierId = material.SupplierId,
                StockQuantity = material.StockQuantity,
                Note = material.Note,
                Status = material.Status,
                CreatedTime = material.CreatedTime
            };

            context.Materials.Add(newMaterial);
            await context.SaveChangesAsync();

            return new MaterialResponse
            {
                Id = newMaterial.Id,
                Code = newMaterial.Code,
                Name = newMaterial.Name,
                CategoryId = newMaterial.CategoryId,
                UnitId = newMaterial.UnitId,
                SupplierId = newMaterial.SupplierId,
                StockQuantity = newMaterial.StockQuantity,
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
                    StockQuantity = m.StockQuantity,
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
                    StockQuantity = m.StockQuantity,
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

            // Validate SupplierId exists
            var supplierExists = await context.Suppliers.AnyAsync(s => s.Id == material.SupplierId);
            if (!supplierExists)
                throw new InvalidOperationException($"Supplier with ID {material.SupplierId} does not exist.");

            existingMaterial.Code = material.Code;
            existingMaterial.Name = material.Name;
            existingMaterial.CategoryId = material.CategoryId;
            existingMaterial.UnitId = material.UnitId;
            existingMaterial.SupplierId = material.SupplierId;
            existingMaterial.StockQuantity = material.StockQuantity;
            existingMaterial.Note = material.Note;
            existingMaterial.Status = material.Status;
            existingMaterial.CreatedTime = material.CreatedTime;

            await context.SaveChangesAsync();
            return true;
        }
    }
}

