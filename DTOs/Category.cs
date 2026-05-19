using WarehouseAPI.Models;

namespace WarehouseAPI.DTOs
{
    public class Category
    {
        public int Id { get; set; }
        public required string CategoryName { get; set; }
        public string Name => CategoryName;
        public bool AssetOnly { get; set; } = false; // true = only for assets, false = only for materials
    }
}
