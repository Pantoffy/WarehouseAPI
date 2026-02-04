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

            var newPurchaseOrder = new PurchaseOrder
            {
                Code = purchaseOrder.Code,
                PoNumber = purchaseOrder.PoNumber,
                OrderDate = purchaseOrder.OrderDate,
                SupplierId = purchaseOrder.SupplierId,
                ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
                TotalAmount = purchaseOrder.TotalAmount,
                Status = purchaseOrder.Status,
                CreatedBy = purchaseOrder.CreatedBy,
                ApprovedBy = purchaseOrder.ApprovedBy,
                ApprovedAt = purchaseOrder.ApprovedAt,
                Note = purchaseOrder.Note,
                CreatedAt = purchaseOrder.CreatedAt
            };

            context.PurchaseOrder.Add(newPurchaseOrder);
            await context.SaveChangesAsync();

            return new PurchaseOrderResponse
            {
                Id = newPurchaseOrder.Id,
                Code = newPurchaseOrder.Code,
                PoNumber = newPurchaseOrder.PoNumber,
                OrderDate = newPurchaseOrder.OrderDate,
                SupplierId = newPurchaseOrder.SupplierId,
                ExpectedDeliveryDate = newPurchaseOrder.ExpectedDeliveryDate,
                TotalAmount = newPurchaseOrder.TotalAmount,
                Status = newPurchaseOrder.Status,
                CreatedBy = newPurchaseOrder.CreatedBy,
                ApprovedBy = newPurchaseOrder.ApprovedBy,
                ApprovedAt = newPurchaseOrder.ApprovedAt,
                Note = newPurchaseOrder.Note,
                CreatedAt = newPurchaseOrder.CreatedAt
            };
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
                    PurchaseOrderDetails = po.PurchaseOrderDetails
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
                    PurchaseOrderDetails = po.PurchaseOrderDetails
                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> UpdatePurchaseOrderByIdAsync(int id, UpdatePurchaseOrderRequest purchaseOrder)
        {
            var existingPurchaseOrder = await context.PurchaseOrder.FindAsync(id);
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

            existingPurchaseOrder.Code = purchaseOrder.Code;
            existingPurchaseOrder.PoNumber = purchaseOrder.PoNumber;
            existingPurchaseOrder.OrderDate = purchaseOrder.OrderDate;
            existingPurchaseOrder.SupplierId = purchaseOrder.SupplierId;
            existingPurchaseOrder.ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate;
            existingPurchaseOrder.TotalAmount = purchaseOrder.TotalAmount;
            existingPurchaseOrder.Status = purchaseOrder.Status;
            existingPurchaseOrder.CreatedBy = purchaseOrder.CreatedBy;
            existingPurchaseOrder.ApprovedBy = purchaseOrder.ApprovedBy;
            existingPurchaseOrder.ApprovedAt = purchaseOrder.ApprovedAt;
            existingPurchaseOrder.Note = purchaseOrder.Note;
            existingPurchaseOrder.CreatedAt = purchaseOrder.CreatedAt;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
