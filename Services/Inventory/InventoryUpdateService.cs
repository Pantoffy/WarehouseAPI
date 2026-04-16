using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
using WarehouseAPI.Models;

namespace WarehouseAPI.Services.Inventory
{
    public interface IInventoryUpdateService
    {
        /// <summary>
        /// Update inventory when import receipt is approved
        /// Increases stock quantity for each material
        /// </summary>
        Task<bool> UpdateInventoryOnImportApprovedAsync(int importReceiptId);

        /// <summary>
        /// Update inventory when import receipt is cancelled
        /// Decreases stock quantity for each material
        /// </summary>
        Task<bool> UpdateInventoryOnImportCancelledAsync(int importReceiptId);

        /// <summary>
        /// Update inventory when export receipt is approved
        /// Decreases stock quantity for each material
        /// </summary>
        Task<bool> UpdateInventoryOnExportApprovedAsync(int exportReceiptId);

        /// <summary>
        /// Revert inventory when export receipt is cancelled
        /// Adds back stock quantity for each material
        /// </summary>
        Task<bool> UpdateInventoryOnExportCancelledAsync(int exportReceiptId);

        /// <summary>
        /// Sync StockQuantity từ Inventory cho một Material
        /// StockQuantity = tổng Quantity từ tất cả kho
        /// </summary>
        Task SyncMaterialStockQuantityAsync(int materialId);

        /// <summary>
        /// Sync StockQuantity cho nhiều Material
        /// </summary>
        Task SyncMaterialStockQuantitiesAsync(IEnumerable<int> materialIds);
    }

    public class InventoryUpdateService(AppDbContext context) : IInventoryUpdateService
    {
        public async Task<bool> UpdateInventoryOnImportApprovedAsync(int importReceiptId)
        {
            var importReceipt = await context.ImportReceipt
                .Include(ir => ir.ImportReceiptDetails)
                .FirstOrDefaultAsync(ir => ir.Id == importReceiptId);

            if (importReceipt is null)
                return false;

            var materialIds = new List<int>();

            // Process each detail
            foreach (var detail in importReceipt.ImportReceiptDetails ?? new List<ImportReceiptDetail>())
            {
                var inventory = await context.Inventory
                    .FirstOrDefaultAsync(i => 
                        i.WarehouseId == importReceipt.WarehouseId && 
                        i.MaterialId == detail.MaterialId);

                if (inventory is not null)
                {
                    // Increase stock (nhập kho)
                    inventory.Quantity += detail.Quantity;
                    inventory.UpdatedDate = DateTime.Now;
                }
                else
                {
                    // Create new inventory if not exists
                    var newInventory = new Models.Inventory
                    {
                        WarehouseId = importReceipt.WarehouseId,
                        MaterialId = detail.MaterialId,
                        Quantity = detail.Quantity,
                        UpdatedDate = DateTime.Now
                    };
                    context.Inventory.Add(newInventory);
                }

                materialIds.Add(detail.MaterialId);
            }

            await context.SaveChangesAsync();

            // Sync StockQuantity cho tất cả Material liên quan
            foreach (var materialId in materialIds.Distinct())
            {
                await SyncMaterialStockQuantityAsync(materialId);
            }

            return true;
        }

        /// <summary>
        /// Update inventory when import receipt is cancelled
        /// Decreases stock quantity for each material
        /// </summary>
        public async Task<bool> UpdateInventoryOnImportCancelledAsync(int importReceiptId)
        {
            var importReceipt = await context.ImportReceipt
                .Include(ir => ir.ImportReceiptDetails)
                .FirstOrDefaultAsync(ir => ir.Id == importReceiptId);

            if (importReceipt is null)
                return false;

            var materialIds = new List<int>();

            // Process each detail - SUBTRACT the quantity
            foreach (var detail in importReceipt.ImportReceiptDetails ?? new List<ImportReceiptDetail>())
            {
                var inventory = await context.Inventory
                    .FirstOrDefaultAsync(i => 
                        i.WarehouseId == importReceipt.WarehouseId && 
                        i.MaterialId == detail.MaterialId);

                if (inventory is null)
                    throw new InvalidOperationException(
                        $"Không tìm thấy hàng tồn kho cho vật liệu {detail.MaterialId} trong kho {importReceipt.WarehouseId}");

                // Decrease quantity (hủy nhập kho)
                inventory.Quantity -= detail.Quantity;
                inventory.UpdatedDate = DateTime.Now;

                if (inventory.Quantity < 0)
                    throw new InvalidOperationException(
                        $"Hàng tồn kho không được âm cho vật liệu {detail.MaterialId}");

                materialIds.Add(detail.MaterialId);
            }

            await context.SaveChangesAsync();

            // Sync StockQuantity cho tất cả Material liên quan
            foreach (var materialId in materialIds.Distinct())
            {
                await SyncMaterialStockQuantityAsync(materialId);
            }

            return true;
        }

        public async Task<bool> UpdateInventoryOnExportApprovedAsync(int exportReceiptId)
        {
            var exportReceipt = await context.ExportReceipt
                .Include(er => er.ExportReceiptDetails)
                .FirstOrDefaultAsync(er => er.Id == exportReceiptId);

            if (exportReceipt is null)
                return false;

            var materialIds = new List<int>();

            // Process each detail
            foreach (var detail in exportReceipt.ExportReceiptDetails ?? new List<ExportReceiptDetail>())
            {
                var inventory = await context.Inventory
                    .FirstOrDefaultAsync(i => 
                        i.WarehouseId == exportReceipt.WarehouseId && 
                        i.MaterialId == detail.MaterialId);

                if (inventory is null)
                    throw new InvalidOperationException(
                        $"Không tìm thấy hàng tồn kho cho vật liệu {detail.MaterialId} trong kho {exportReceipt.WarehouseId}");

                // Decrease stock (xuất kho)
                inventory.Quantity -= detail.Quantity;
                inventory.UpdatedDate = DateTime.Now;

                if (inventory.Quantity < 0)
                    throw new InvalidOperationException(
                        $"Hàng tồn kho không được âm cho vật liệu {detail.MaterialId}");

                materialIds.Add(detail.MaterialId);
            }

            await context.SaveChangesAsync();

            // Sync StockQuantity cho tất cả Material liên quan
            foreach (var materialId in materialIds.Distinct())
            {
                await SyncMaterialStockQuantityAsync(materialId);
            }

            return true;
        }

        /// <summary>
        /// Revert inventory when export receipt is cancelled
        /// Adds back stock quantity for each material
        /// </summary>
        public async Task<bool> UpdateInventoryOnExportCancelledAsync(int exportReceiptId)
        {
            var exportReceipt = await context.ExportReceipt
                .Include(er => er.ExportReceiptDetails)
                .FirstOrDefaultAsync(er => er.Id == exportReceiptId);

            if (exportReceipt is null)
                return false;

            var materialIds = new List<int>();

            // Process each detail - ADD BACK the quantity
            foreach (var detail in exportReceipt.ExportReceiptDetails ?? new List<ExportReceiptDetail>())
            {
                var inventory = await context.Inventory
                    .FirstOrDefaultAsync(i => 
                        i.WarehouseId == exportReceipt.WarehouseId && 
                        i.MaterialId == detail.MaterialId);

                if (inventory is null)
                    throw new InvalidOperationException(
                        $"Không tìm thấy hàng tồn kho cho vật liệu {detail.MaterialId} trong kho {exportReceipt.WarehouseId}");

                // Add back quantity (hoàn hàng khi hủy xuất)
                inventory.Quantity += detail.Quantity;
                inventory.UpdatedDate = DateTime.Now;

                materialIds.Add(detail.MaterialId);
            }

            await context.SaveChangesAsync();

            // Sync StockQuantity cho tất cả Material liên quan
            foreach (var materialId in materialIds.Distinct())
            {
                await SyncMaterialStockQuantityAsync(materialId);
            }

            return true;
        }

        /// <summary>
        /// Sync StockQuantity từ Inventory cho một Material
        /// StockQuantity = tổng Quantity từ tất cả kho
        /// </summary>
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

        /// <summary>
        /// Sync StockQuantity cho nhiều Material
        /// </summary>
        public async Task SyncMaterialStockQuantitiesAsync(IEnumerable<int> materialIds)
        {
            foreach (var materialId in materialIds.Distinct())
            {
                await SyncMaterialStockQuantityAsync(materialId);
            }
        }
    }
}
