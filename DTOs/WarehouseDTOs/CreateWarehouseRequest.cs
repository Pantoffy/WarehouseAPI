namespace WarehouseAPI.DTOs.WarehouseDTOs
{
    public class CreateWarehouseRequest
    {
        public required string Code { get; set; }
        public required string Name { get; set; }

        public int TypeId { get; set; }

        public string? Address { get; set; }
        public decimal? Area { get; set; }

        public string? ManagerName { get; set; }
        public string? ManagerPhone { get; set; }

        public string? Status { get; set; }
        public string? Note { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}
