using WarehouseAPI.DTOs.PurchaseOrderDTOs;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Services.PurchaseOrder
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private IUOW UOW;

        public PurchaseOrderService(IUOW uow)
        {
            UOW = uow;
        }

        public async Task<PurchaseOrderResponse> AddPurchaseOrderAsync(CreatePurchaseOrderRequest purchaseOrder)
        {
            return await UOW.PurchaseOrderRepository.AddPurchaseOrderAsync(purchaseOrder);
        }

        public async Task<bool> DeletePurchaseOrderByIdAsync(int id)
        {
            return await UOW.PurchaseOrderRepository.DeletePurchaseOrderByIdAsync(id);
        }

        public async Task<List<PurchaseOrderResponse>> GetAllPurchaseOrderAsync()
        {
            return await UOW.PurchaseOrderRepository.GetAllPurchaseOrderAsync();
        }

        public async Task<PurchaseOrderResponse?> GetPurchaseOrderByIdAsync(int id)
        {
            return await UOW.PurchaseOrderRepository.GetPurchaseOrderByIdAsync(id);
        }

        public async Task<bool> UpdatePurchaseOrderByIdAsync(int id, UpdatePurchaseOrderRequest purchaseOrder)
        {
            return await UOW.PurchaseOrderRepository.UpdatePurchaseOrderByIdAsync(id, purchaseOrder);
        }
    }
}
