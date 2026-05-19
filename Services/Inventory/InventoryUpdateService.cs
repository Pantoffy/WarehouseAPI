using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
using WarehouseAPI.DTOs;
using WarehouseAPI.Models;

namespace WarehouseAPI.Services.Inventory
{
    public interface IInventoryUpdateService
    {
        Task<bool> UpdateInventoryOnImportApprovedAsync(int importReceiptId);
        Task<bool> UpdateInventoryOnImportCancelledAsync(int importReceiptId);
        Task<bool> UpdateInventoryOnExportApprovedAsync(int exportReceiptId);
        Task<bool> UpdateInventoryOnExportCancelledAsync(int exportReceiptId);
    }

    public class InventoryUpdateService(AppDbContext context) : IInventoryUpdateService
    {
        public async Task<bool> UpdateInventoryOnImportApprovedAsync(int importReceiptId)
        {
            var importReceipt = await context.ImportReceipt
                .Include(ir => ir.ImportReceiptDetails)
                .ThenInclude(d => d.Material)
                .FirstOrDefaultAsync(ir => ir.Id == importReceiptId);

            if (importReceipt is null)
                return false;

            foreach (var detail in importReceipt.ImportReceiptDetails ?? new List<ImportReceiptDetail>())
            {
                var materialUnitId = detail.Material?.UnitId ?? detail.UnitId;
                var normalizedQuantity = ConvertToMaterialBaseUnit(detail.Quantity, detail.UnitId, materialUnitId, detail.MaterialId);

                var inventory = await context.Inventory
                    .FirstOrDefaultAsync(i =>
                        i.WarehouseId == importReceipt.WarehouseId &&
                        i.MaterialId == detail.MaterialId);

                if (inventory is not null)
                {
                    inventory.Quantity += normalizedQuantity;
                    inventory.UpdatedDate = DateTime.Now;
                }
                else
                {
                    context.Inventory.Add(new Models.Inventory
                    {
                        WarehouseId = importReceipt.WarehouseId,
                        MaterialId = detail.MaterialId,
                        Quantity = normalizedQuantity,
                        UpdatedDate = DateTime.Now
                    });
                }
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateInventoryOnImportCancelledAsync(int importReceiptId)
        {
            var importReceipt = await context.ImportReceipt
                .Include(ir => ir.ImportReceiptDetails)
                .ThenInclude(d => d.Material)
                .FirstOrDefaultAsync(ir => ir.Id == importReceiptId);

            if (importReceipt is null)
                return false;

            foreach (var detail in importReceipt.ImportReceiptDetails ?? new List<ImportReceiptDetail>())
            {
                var materialUnitId = detail.Material?.UnitId ?? detail.UnitId;
                var normalizedQuantity = ConvertToMaterialBaseUnit(detail.Quantity, detail.UnitId, materialUnitId, detail.MaterialId);

                var inventory = await context.Inventory
                    .FirstOrDefaultAsync(i =>
                        i.WarehouseId == importReceipt.WarehouseId &&
                        i.MaterialId == detail.MaterialId);

                if (inventory is null)
                    throw new InvalidOperationException($"Không tìm thấy hàng tồn kho cho vật liệu {detail.MaterialId} trong kho {importReceipt.WarehouseId}");

                inventory.Quantity -= normalizedQuantity;
                inventory.UpdatedDate = DateTime.Now;

                if (inventory.Quantity < 0)
                    throw new InvalidOperationException($"Hàng tồn kho không được âm cho vật liệu {detail.MaterialId}");
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateInventoryOnExportApprovedAsync(int exportReceiptId)
        {
            var exportReceipt = await context.ExportReceipt
                .Include(er => er.ExportReceiptDetails)
                .ThenInclude(d => d.Material)
                .FirstOrDefaultAsync(er => er.Id == exportReceiptId);

            if (exportReceipt is null)
                return false;

            foreach (var detail in exportReceipt.ExportReceiptDetails ?? new List<ExportReceiptDetail>())
            {
                var materialUnitId = detail.Material?.UnitId ?? detail.UnitId;
                var normalizedQuantity = ConvertToMaterialBaseUnit(detail.Quantity, detail.UnitId, materialUnitId, detail.MaterialId);

                var inventory = await context.Inventory
                    .FirstOrDefaultAsync(i =>
                        i.WarehouseId == exportReceipt.WarehouseId &&
                        i.MaterialId == detail.MaterialId);

                if (inventory is null)
                    throw new InvalidOperationException($"Không tìm thấy hàng tồn kho cho vật liệu {detail.MaterialId} trong kho {exportReceipt.WarehouseId}");

                inventory.Quantity -= normalizedQuantity;
                inventory.UpdatedDate = DateTime.Now;

                if (inventory.Quantity < 0)
                    throw new InvalidOperationException($"Hàng tồn kho không được âm cho vật liệu {detail.MaterialId}");
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateInventoryOnExportCancelledAsync(int exportReceiptId)
        {
            var exportReceipt = await context.ExportReceipt
                .Include(er => er.ExportReceiptDetails)
                .ThenInclude(d => d.Material)
                .FirstOrDefaultAsync(er => er.Id == exportReceiptId);

            if (exportReceipt is null)
                return false;

            foreach (var detail in exportReceipt.ExportReceiptDetails ?? new List<ExportReceiptDetail>())
            {
                var materialUnitId = detail.Material?.UnitId ?? detail.UnitId;
                var normalizedQuantity = ConvertToMaterialBaseUnit(detail.Quantity, detail.UnitId, materialUnitId, detail.MaterialId);

                var inventory = await context.Inventory
                    .FirstOrDefaultAsync(i =>
                        i.WarehouseId == exportReceipt.WarehouseId &&
                        i.MaterialId == detail.MaterialId);

                if (inventory is null)
                    throw new InvalidOperationException($"Không tìm thấy hàng tồn kho cho vật liệu {detail.MaterialId} trong kho {exportReceipt.WarehouseId}");

                inventory.Quantity += normalizedQuantity;
                inventory.UpdatedDate = DateTime.Now;
            }

            await context.SaveChangesAsync();
            return true;
        }

        private static decimal ConvertToMaterialBaseUnit(decimal quantity, int detailUnitId, int materialUnitId, int materialId)
        {
            if (!UnitConversionHelper.TryConvert(quantity, detailUnitId, materialUnitId, out var result, materialId))
                return quantity;

            return result;
        }
    }
}
