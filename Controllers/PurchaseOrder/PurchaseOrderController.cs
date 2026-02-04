using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.DTOs.PurchaseOrderDTOs;
using WarehouseAPI.Services.PurchaseOrder;

namespace WarehouseAPI.Controllers.PurchaseOrder
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController(IPurchaseOrderService service) : ControllerBase
    {
        //hien danh sach don dat hang
        [Route(PurchaseOrderRouter.GetAllPurchaseOrders), HttpGet]
        public async Task<ActionResult<List<PurchaseOrderResponse>>> GetPurchaseOrders()
            => Ok(await service.GetAllPurchaseOrderAsync());

        [Route(PurchaseOrderRouter.GetPurchaseOrderById), HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrderResponse?>> GetPurchaseOrder(int id)
        {
            var purchaseOrder = await service.GetPurchaseOrderByIdAsync(id);
            return purchaseOrder is null ? NotFound("Không tìm thấy đơn đặt hàng với Id đã cho.") : Ok(purchaseOrder);
        }

        //them moi don dat hang
        [Route(PurchaseOrderRouter.AddPurchaseOrder), HttpPost]
        public async Task<ActionResult<PurchaseOrderResponse>> AddPurchaseOrder(CreatePurchaseOrderRequest purchaseOrder)
        {
            try
            {
                var createdPurchaseOrder = await service.AddPurchaseOrderAsync(purchaseOrder);
                return CreatedAtAction(nameof(GetPurchaseOrder), new { id = createdPurchaseOrder.Id }, createdPurchaseOrder);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //cap nhat thong tin don dat hang
        [Route(PurchaseOrderRouter.UpdatePurchaseOrder), HttpPut("{id}")]
        public async Task<ActionResult> UpdatePurchaseOrder(int id, UpdatePurchaseOrderRequest purchaseOrder)
        {
            try
            {
                var updated = await service.UpdatePurchaseOrderByIdAsync(id, purchaseOrder);
                return updated ? Ok("Cập nhật thành công") : NotFound("Không tìm thấy đơn đặt hàng với Id đã cho.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //xoa don dat hang
        [Route(PurchaseOrderRouter.DeletePurchaseOrder), HttpDelete("{id}")]
        public async Task<ActionResult> DeletePurchaseOrder(int id)
        {
            var deleted = await service.DeletePurchaseOrderByIdAsync(id);
            return deleted ? Ok("Xóa đơn đặt hàng thành công") : NotFound("Không tìm thấy đơn đặt hàng với Id đã cho.");
        }
    }
}
