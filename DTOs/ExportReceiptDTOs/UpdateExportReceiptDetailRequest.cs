using System.ComponentModel.DataAnnotations;

namespace WarehouseAPI.DTOs.ExportReceiptDTOs
{
    public class UpdateExportReceiptDetailRequest
    {
        public int? Id { get; set; } // Nullable để phân biệt giữa detail mới và detail đã tồn tại

        [Range(1, int.MaxValue, ErrorMessage = "Mã vật liệu không hợp lệ")]
        public int MaterialId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã đơn vị không hợp lệ")]
        public int UnitId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không hợp lệ")]
        public decimal? UnitPrice { get; set; }

        public string? Note { get; set; }
    }
}
