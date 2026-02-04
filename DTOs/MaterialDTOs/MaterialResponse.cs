using WarehouseAPI.Models;

namespace WarehouseAPI.DTOs.MaterialDTOs
{
    public class MaterialResponse
    {
        public int Id { get; set; }

        public required string Code { get; set; }
        public required string Name { get; set; }

        public int CategoryId { get; set; }
        public int UnitId { get; set; }
        public int SupplierId { get; set; }

        public decimal StockQuantity { get; set; } = 0;

        public string? Note { get; set; }
        public string? Status { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public Supplier? Supplier { get; set; }
    }
}