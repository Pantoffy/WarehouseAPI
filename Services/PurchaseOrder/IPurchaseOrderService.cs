using WarehouseAPI.DTOs.PurchaseOrderDTOs;

namespace WarehouseAPI.Services.PurchaseOrder
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrderResponse>> GetAllPurchaseOrderAsync();
        Task<PurchaseOrderResponse?> GetPurchaseOrderByIdAsync(int id);
        Task<PurchaseOrderResponse> AddPurchaseOrderAsync(CreatePurchaseOrderRequest purchaseOrder);
        Task<bool> UpdatePurchaseOrderByIdAsync(int id, UpdatePurchaseOrderRequest purchaseOrder);
        Task<bool> DeletePurchaseOrderByIdAsync(int id);
    }
}
