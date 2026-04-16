using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
using WarehouseAPI.DTOs.InventoryDTOs;
using WarehouseAPI.Models;

namespace WarehouseAPI.Repository
{
    public interface IInventoryRepository
    {
        Task<List<InventoryResponse>> GetAllInventoryAsync();
        Task<InventoryResponse?> GetInventoryByIdAsync(int id);
        Task<InventoryResponse> AddInventoryAsync(CreateInventoryRequest inventory);
        Task<bool> UpdateInventoryByIdAsync(int id, UpdateInventoryRequest inventory);
        Task<bool> DeleteInventoryByIdAsync(int id);
        Task SyncMaterialStockQuantityAsync(int materialId);
    }

    public class InventoryRepository(AppDbContext context) : IInventoryRepository
    {
        public async Task<InventoryResponse> AddInventoryAsync(CreateInventoryRequest inventory)
        {
            // Validate WarehouseId exists
            var warehouseExists = await context.Warehouse.AnyAsync(w => w.Id == inventory.WarehouseId);
            if (!warehouseExists)
                throw new InvalidOperationException($"Warehouse with ID {inventory.WarehouseId} does not exist.");

            // Validate MaterialId exists
            var materialExists = await context.Materials.AnyAsync(m => m.Id == inventory.MaterialId);
            if (!materialExists)
                throw new InvalidOperationException($"Material with ID {inventory.MaterialId} does not exist.");

            // Check if inventory already exists for this warehouse-material combination
            var existingInventory = await context.Inventory
                .FirstOrDefaultAsync(i => i.WarehouseId == inventory.WarehouseId && i.MaterialId == inventory.MaterialId);
            if (existingInventory != null)
                throw new InvalidOperationException($"Inventory already exists for Warehouse ID {inventory.WarehouseId} and Material ID {inventory.MaterialId}.");

            var newInventory = new Inventory
            {
                WarehouseId = inventory.WarehouseId,
                MaterialId = inventory.MaterialId,
                Quantity = inventory.Quantity,
                UpdatedDate = inventory.UpdatedDate
            };

            context.Inventory.Add(newInventory);
            await context.SaveChangesAsync();

            // Sync Material StockQuantity
            await SyncMaterialStockQuantityAsync(newInventory.MaterialId);

            return new InventoryResponse
            {
                Id = newInventory.Id,
                WarehouseId = newInventory.WarehouseId,
                MaterialId = newInventory.MaterialId,
                Quantity = newInventory.Quantity,
                UpdatedDate = newInventory.UpdatedDate
            };
        }

        public async Task<bool> DeleteInventoryByIdAsync(int id)
        {
            var inventory = await context.Inventory.FindAsync(id);
            if (inventory is null)
                return false;

            var materialId = inventory.MaterialId;
            context.Inventory.Remove(inventory);
            await context.SaveChangesAsync();

            // Sync Material StockQuantity
            await SyncMaterialStockQuantityAsync(materialId);

            return true;
        }

        public async Task<List<InventoryResponse>> GetAllInventoryAsync()
            => await context.Inventory
                .Include(i => i.Warehouse)
                .Include(i => i.Material)
                    .ThenInclude(m => m!.Supplier)
                .Select(i => new InventoryResponse
                {
                    Id = i.Id,
                    WarehouseId = i.WarehouseId,
                    MaterialId = i.MaterialId,
                    Quantity = i.Quantity,
                    UpdatedDate = i.UpdatedDate,
                    Warehouse = i.Warehouse,
                    Material = i.Material
                }).ToListAsync();

        public async Task<InventoryResponse?> GetInventoryByIdAsync(int id)
        {
            var result = await context.Inventory
                .Include(i => i.Warehouse)
                .Include(i => i.Material)
                    .ThenInclude(m => m!.Supplier)
                .Where(i => i.Id == id)
                .Select(i => new InventoryResponse
                {
                    Id = i.Id,
                    WarehouseId = i.WarehouseId,
                    MaterialId = i.MaterialId,
                    Quantity = i.Quantity,
                    UpdatedDate = i.UpdatedDate,
                    Warehouse = i.Warehouse,
                    Material = i.Material
                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> UpdateInventoryByIdAsync(int id, UpdateInventoryRequest inventory)
        {
            var existingInventory = await context.Inventory.FindAsync(id);
            if (existingInventory is null)
                return false;

            // Validate WarehouseId exists
            var warehouseExists = await context.Warehouse.AnyAsync(w => w.Id == inventory.WarehouseId);
            if (!warehouseExists)
                throw new InvalidOperationException($"Warehouse with ID {inventory.WarehouseId} does not exist.");

            // Validate MaterialId exists
            var materialExists = await context.Materials.AnyAsync(m => m.Id == inventory.MaterialId);
            if (!materialExists)
                throw new InvalidOperationException($"Material with ID {inventory.MaterialId} does not exist.");

            // Check if updating would create a duplicate warehouse-material combination
            var duplicateExists = await context.Inventory
                .AnyAsync(i => i.Id != id && i.WarehouseId == inventory.WarehouseId && i.MaterialId == inventory.MaterialId);
            if (duplicateExists)
                throw new InvalidOperationException($"Inventory already exists for Warehouse ID {inventory.WarehouseId} and Material ID {inventory.MaterialId}.");

            var previousMaterialId = existingInventory.MaterialId;
            existingInventory.WarehouseId = inventory.WarehouseId;
            existingInventory.MaterialId = inventory.MaterialId;
            existingInventory.Quantity = inventory.Quantity;
            existingInventory.UpdatedDate = inventory.UpdatedDate;

            await context.SaveChangesAsync();

            // Sync Material StockQuantity - nếu Material thay đổi, sync cả 2
            if (previousMaterialId != inventory.MaterialId)
            {
                await SyncMaterialStockQuantityAsync(previousMaterialId);
            }
            await SyncMaterialStockQuantityAsync(inventory.MaterialId);

            return true;
        }

        public async Task SyncMaterialStockQuantityAsync(int materialId)
        {
            var material = await context.Materials.FindAsync(materialId);
            if (material is null)
                return;

            // Tính tổng số lượng từ tất cả kho
            var totalQuantity = await context.Inventory
                .Where(i => i.MaterialId == materialId)
                .SumAsync(i => i.Quantity);

            material.StockQuantity = totalQuantity;
            await context.SaveChangesAsync();
        }
    }
}
