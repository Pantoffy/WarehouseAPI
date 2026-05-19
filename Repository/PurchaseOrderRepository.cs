using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
using WarehouseAPI.DTOs.PurchaseOrderDTOs;
using WarehouseAPI.Models;

namespace WarehouseAPI.Repository
{
    public interface IPurchaseOrderRepository
    {
        Task<List<PurchaseOrderResponse>> GetAllPurchaseOrderAsync();
        Task<PurchaseOrderResponse?> GetPurchaseOrderByIdAsync(int id);
        Task<PurchaseOrderResponse> AddPurchaseOrderAsync(CreatePurchaseOrderRequest purchaseOrder);
        Task<bool> UpdatePurchaseOrderByIdAsync(int id, UpdatePurchaseOrderRequest purchaseOrder);
        Task<bool> DeletePurchaseOrderByIdAsync(int id);
    }

    public class PurchaseOrderRepository(AppDbContext context) : IPurchaseOrderRepository
    {
        public async Task<PurchaseOrderResponse> AddPurchaseOrderAsync(CreatePurchaseOrderRequest purchaseOrder)
        {
            // Validate SupplierId exists
            var supplierExists = await context.Suppliers.AnyAsync(s => s.Id == purchaseOrder.SupplierId);
            if (!supplierExists)
                throw new InvalidOperationException($"Supplier with ID {purchaseOrder.SupplierId} does not exist.");

            // Check if PoNumber already exists
            var poExists = await context.PurchaseOrder.AnyAsync(po => po.PoNumber == purchaseOrder.PoNumber);
            if (poExists)
                throw new InvalidOperationException($"Purchase Order number '{purchaseOrder.PoNumber}' already exists.");

            // Validate PurchaseOrderDetails materials exist
            if (purchaseOrder.PurchaseOrderDetails?.Any() == true)
            {
                var materialIds = purchaseOrder.PurchaseOrderDetails.Select(d => d.MaterialId).Distinct();
                var existingMaterials = await context.Materials.Where(m => materialIds.Contains(m.Id)).Select(m => m.Id).ToListAsync();

                foreach (var materialId in materialIds)
                {
                    if (!existingMaterials.Contains(materialId))
                        throw new InvalidOperationException($"Material with ID {materialId} does not exist.");
                }
            }

            var normalizedStatus = NormalizePurchaseOrderStatus(purchaseOrder.Status);

            var newPurchaseOrder = new PurchaseOrder
            {
                Code = purchaseOrder.Code,
                PoNumber = purchaseOrder.PoNumber,
                OrderDate = purchaseOrder.OrderDate,
                SupplierId = purchaseOrder.SupplierId,
                ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
                TotalAmount = purchaseOrder.TotalAmount,
                Status = normalizedStatus,
                CreatedBy = purchaseOrder.CreatedBy,
                ApprovedBy = purchaseOrder.ApprovedBy,
                ApprovedAt = purchaseOrder.ApprovedAt,
                Note = purchaseOrder.Note,
                CreatedAt = purchaseOrder.CreatedAt
            };

            context.PurchaseOrder.Add(newPurchaseOrder);
            await context.SaveChangesAsync();

            // Add PurchaseOrderDetails if provided
            if (purchaseOrder.PurchaseOrderDetails?.Any() == true)
            {
                var details = purchaseOrder.PurchaseOrderDetails.Select(d => new PurchaseOrderDetail
                {
                    PurchaseOrderId = newPurchaseOrder.Id,
                    MaterialId = d.MaterialId,
                    UnitId = d.UnitId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Amount = d.Quantity * d.UnitPrice, // Auto-calculate
                    Note = d.Note
                }).ToList();

                context.PurchaseOrderDetail.AddRange(details);
                await context.SaveChangesAsync();
            }

            // Reload full data with related entities to return
            var result = await context.PurchaseOrder
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderDetails!)
                    .ThenInclude(pod => pod.Material)
                .Where(po => po.Id == newPurchaseOrder.Id)
                .Select(po => new PurchaseOrderResponse
                {
                    Id = po.Id,
                    Code = po.Code,
                    PoNumber = po.PoNumber,
                    OrderDate = po.OrderDate,
                    SupplierId = po.SupplierId,
                    ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                    TotalAmount = po.TotalAmount,
                    Status = po.Status,
                    CreatedBy = po.CreatedBy,
                    ApprovedBy = po.ApprovedBy,
                    ApprovedAt = po.ApprovedAt,
                    Note = po.Note,
                    CreatedAt = po.CreatedAt,
                    Supplier = po.Supplier,
                    PurchaseOrderDetails = po.PurchaseOrderDetails!.Select(pod => new PurchaseOrderDetailResponse
                    {
                        Id = pod.Id,
                        PurchaseOrderId = pod.PurchaseOrderId,
                        MaterialId = pod.MaterialId,
                        UnitId = pod.UnitId,
                        Quantity = pod.Quantity,
                        UnitPrice = pod.UnitPrice,
                        Amount = pod.Amount,
                        Note = pod.Note,
                        Material = pod.Material
                    }).ToList()
                }).FirstOrDefaultAsync();

            return result!;
        }

        public async Task<bool> DeletePurchaseOrderByIdAsync(int id)
        {
            var purchaseOrder = await context.PurchaseOrder.FindAsync(id);
            if (purchaseOrder is null)
                return false;

            context.PurchaseOrder.Remove(purchaseOrder);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PurchaseOrderResponse>> GetAllPurchaseOrderAsync()
            => await context.PurchaseOrder
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderDetails!)
                    .ThenInclude(pod => pod.Material)
                .Select(po => new PurchaseOrderResponse
                {
                    Id = po.Id,
                    Code = po.Code,
                    PoNumber = po.PoNumber,
                    OrderDate = po.OrderDate,
                    SupplierId = po.SupplierId,
                    ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                    TotalAmount = po.TotalAmount,
                    Status = po.Status,
                    CreatedBy = po.CreatedBy,
                    ApprovedBy = po.ApprovedBy,
                    ApprovedAt = po.ApprovedAt,
                    Note = po.Note,
                    CreatedAt = po.CreatedAt,
                    Supplier = po.Supplier,
                    PurchaseOrderDetails = po.PurchaseOrderDetails!.Select(pod => new PurchaseOrderDetailResponse
                    {
                        Id = pod.Id,
                        PurchaseOrderId = pod.PurchaseOrderId,
                        MaterialId = pod.MaterialId,
                        UnitId = pod.UnitId,
                        Quantity = pod.Quantity,
                        UnitPrice = pod.UnitPrice,
                        Amount = pod.Amount,
                        Note = pod.Note,
                        Material = pod.Material
                    }).ToList()
                }).ToListAsync();

        public async Task<PurchaseOrderResponse?> GetPurchaseOrderByIdAsync(int id)
        {
            var result = await context.PurchaseOrder
                .Include(po => po.Supplier)
                .Include(po => po.PurchaseOrderDetails!)
                    .ThenInclude(pod => pod.Material)
                .Where(po => po.Id == id)
                .Select(po => new PurchaseOrderResponse
                {
                    Id = po.Id,
                    Code = po.Code,
                    PoNumber = po.PoNumber,
                    OrderDate = po.OrderDate,
                    SupplierId = po.SupplierId,
                    ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                    TotalAmount = po.TotalAmount,
                    Status = po.Status,
                    CreatedBy = po.CreatedBy,
                    ApprovedBy = po.ApprovedBy,
                    ApprovedAt = po.ApprovedAt,
                    Note = po.Note,
                    CreatedAt = po.CreatedAt,
                    Supplier = po.Supplier,
                    PurchaseOrderDetails = po.PurchaseOrderDetails!.Select(pod => new PurchaseOrderDetailResponse
                    {
                        Id = pod.Id,
                        PurchaseOrderId = pod.PurchaseOrderId,
                        MaterialId = pod.MaterialId,
                        UnitId = pod.UnitId,
                        Quantity = pod.Quantity,
                        UnitPrice = pod.UnitPrice,
                        Amount = pod.Amount,
                        Note = pod.Note,
                        Material = pod.Material
                    }).ToList()
                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> UpdatePurchaseOrderByIdAsync(int id, UpdatePurchaseOrderRequest purchaseOrder)
        {
            var existingPurchaseOrder = await context.PurchaseOrder
                .Include(po => po.PurchaseOrderDetails)
                .FirstOrDefaultAsync(po => po.Id == id);
            if (existingPurchaseOrder is null)
                return false;

            // Validate SupplierId exists
            var supplierExists = await context.Suppliers.AnyAsync(s => s.Id == purchaseOrder.SupplierId);
            if (!supplierExists)
                throw new InvalidOperationException($"Supplier with ID {purchaseOrder.SupplierId} does not exist.");

            // Check if PoNumber already exists (excluding current record)
            var poExists = await context.PurchaseOrder.AnyAsync(po => po.Id != id && po.PoNumber == purchaseOrder.PoNumber);
            if (poExists)
                throw new InvalidOperationException($"Purchase Order number '{purchaseOrder.PoNumber}' already exists.");

            // Validate PurchaseOrderDetails materials exist
            if (purchaseOrder.PurchaseOrderDetails?.Any() == true)
            {
                var materialIds = purchaseOrder.PurchaseOrderDetails.Select(d => d.MaterialId).Distinct();
                var existingMaterials = await context.Materials.Where(m => materialIds.Contains(m.Id)).Select(m => m.Id).ToListAsync();

                foreach (var materialId in materialIds)
                {
                    if (!existingMaterials.Contains(materialId))
                        throw new InvalidOperationException($"Material with ID {materialId} does not exist.");
                }
            }

            existingPurchaseOrder.Code = purchaseOrder.Code;
            existingPurchaseOrder.PoNumber = purchaseOrder.PoNumber;
            existingPurchaseOrder.OrderDate = purchaseOrder.OrderDate;
            existingPurchaseOrder.SupplierId = purchaseOrder.SupplierId;
            existingPurchaseOrder.ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate;
            existingPurchaseOrder.TotalAmount = purchaseOrder.TotalAmount;
            existingPurchaseOrder.Status = NormalizePurchaseOrderStatus(purchaseOrder.Status);
            if (!string.IsNullOrEmpty(purchaseOrder.CreatedBy))
                existingPurchaseOrder.CreatedBy = purchaseOrder.CreatedBy;
            if (!string.IsNullOrEmpty(purchaseOrder.ApprovedBy))
                existingPurchaseOrder.ApprovedBy = purchaseOrder.ApprovedBy;
            if (purchaseOrder.ApprovedAt.HasValue)
                existingPurchaseOrder.ApprovedAt = purchaseOrder.ApprovedAt;
            existingPurchaseOrder.Note = purchaseOrder.Note;
            if (purchaseOrder.CreatedAt.HasValue && purchaseOrder.CreatedAt.Value > DateTime.MinValue)
            {
                existingPurchaseOrder.CreatedAt = purchaseOrder.CreatedAt.Value;
            }

            // Handle PurchaseOrderDetails (update/add/delete)
            if (purchaseOrder.PurchaseOrderDetails?.Any() == true)
            {
                // Xóa chi tiết cũ
                context.PurchaseOrderDetail.RemoveRange(existingPurchaseOrder.PurchaseOrderDetails ?? new List<PurchaseOrderDetail>());

                // Thêm chi tiết mới
                var details = purchaseOrder.PurchaseOrderDetails.Select(d => new PurchaseOrderDetail
                {
                    PurchaseOrderId = id,
                    MaterialId = d.MaterialId,
                    UnitId = d.UnitId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Amount = d.Quantity * d.UnitPrice, // Auto-calculate
                    Note = d.Note
                }).ToList();

                context.PurchaseOrderDetail.AddRange(details);
            }
            else
            {
                // Nếu không có details, xóa tất cả details cũ
                context.PurchaseOrderDetail.RemoveRange(existingPurchaseOrder.PurchaseOrderDetails ?? new List<PurchaseOrderDetail>());
            }

            await context.SaveChangesAsync();
            return true;
        }

        private static string NormalizePurchaseOrderStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new InvalidOperationException("Trạng thái đơn đặt hàng là bắt buộc.");

            var value = status.Trim();

            if (value.Equals("Đang soạn thảo", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Draft", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Nháp", StringComparison.OrdinalIgnoreCase))
            {
                return "Đang soạn thảo";
            }

            if (value.Equals("Chờ xác nhận", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Pending", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Chờ duyệt", StringComparison.OrdinalIgnoreCase))
            {
                return "Chờ xác nhận";
            }

            if (value.Equals("Đã xác nhận", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Approved", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Đã duyệt", StringComparison.OrdinalIgnoreCase))
            {
                return "Đã xác nhận";
            }

            if (value.Equals("Đã giao hàng", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Delivered", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Hoàn thành", StringComparison.OrdinalIgnoreCase))
            {
                return "Đã giao hàng";
            }

            throw new InvalidOperationException($"Trạng thái đơn đặt hàng không hợp lệ: {status}");
        }
    }
}
