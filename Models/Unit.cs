namespace WarehouseAPI.Models
{
    public class Unit
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool AllowDecimal { get; set; }
    }
}

