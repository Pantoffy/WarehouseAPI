using System.ComponentModel.DataAnnotations;
using WarehouseAPI.DTOs.Validators;

namespace WarehouseAPI.DTOs.ExportReceiptDTOs
{
    public class CreateExportReceiptDetailRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Mã vật liệu không hợp lệ")]
        public int MaterialId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã đơn vị không hợp lệ")]
        public int UnitId { get; set; }

        [ValidateQuantity]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không hợp lệ")]
        public decimal? UnitPrice { get; set; }

        public string? Note { get; set; }
    }
}
