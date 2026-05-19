using System.ComponentModel.DataAnnotations;

namespace WarehouseAPI.DTOs.StockDTOs
{
    public class CreateStockTeamRequest
    {
        public int StockCheckId { get; set; }

        [Required(ErrorMessage = "Tên thành viên/nhóm là bắt buộc")]
        [MinLength(1, ErrorMessage = "Tên thành viên/nhóm không được để trống")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [MinLength(1, ErrorMessage = "Vai trò không được để trống")]
        public required string Role { get; set; }

        public string? Note { get; set; }
    }
}
