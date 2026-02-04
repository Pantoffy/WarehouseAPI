namespace WarehouseAPI.DTOs.SupplierDTOs
{
    public class UpdateSupplierRequest
    {
        public required string Code { get; set; }
        public required string Type { get; set; }

        public string? Name { get; set; }
        public string? ContactPerson { get; set; }
        public string? Title { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }

        public string? Role { get; set; }
        public string? CitizenId { get; set; }

        public string? Address { get; set; }

        public required string Status { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}
