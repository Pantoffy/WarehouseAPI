namespace WarehouseAPI.DTOs.MaterialDTOs
{
    public class CreateMaterialRequest
    {
        public required string Code { get; set; }
        public required string Name { get; set; }

        public int CategoryId { get; set; }
        public int UnitId { get; set; }
        public int SupplierId { get; set; }

        public string? ItemType { get; set; }

        public string? Note { get; set; }
        public string? Status { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}
