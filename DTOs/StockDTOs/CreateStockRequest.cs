using System.ComponentModel.DataAnnotations;

namespace WarehouseAPI.DTOs.StockDTOs
{
    public class CreateStockRequest
    {
        [Required(ErrorMessage = "Mã phiếu kiểm là bắt buộc")]
        [MinLength(1, ErrorMessage = "Mã phiếu kiểm không được để trống")]
        public required string Code { get; set; }

        [Required(ErrorMessage = "Tên phiếu kiểm là bắt buộc")]
        [MinLength(1, ErrorMessage = "Tên phiếu kiểm không được để trống")]
        public required string Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã kho không hợp lệ")]
        public int WarehouseId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Note { get; set; }

        [Required(ErrorMessage = "Phiếu kiểm phải có ít nhất một chi tiết")]
        [MinLength(1, ErrorMessage = "Phiếu kiểm phải có ít nhất một chi tiết")]
        public required CreateStockDetailRequest[] StockCheckDetails { get; set; }

        [Required(ErrorMessage = "Phiếu kiểm phải có ít nhất một đội kiểm")]
        [MinLength(1, ErrorMessage = "Phiếu kiểm phải có ít nhất một đội kiểm")]
        public required CreateStockTeamRequest[] Teams { get; set; }
    }
}
