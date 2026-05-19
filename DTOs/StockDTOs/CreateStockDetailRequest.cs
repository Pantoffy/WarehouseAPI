using System.ComponentModel.DataAnnotations;

namespace WarehouseAPI.DTOs.StockDTOs
{
    public class CreateStockDetailRequest
    {
        public int StockCheckId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã vật tư không hợp lệ")]
        public int MaterialId { get; set; }

        public decimal SystemQuantity { get; set; }
        public decimal ActualQuantity { get; set; }
        public string? HandlingProposal { get; set; }
    }
}
