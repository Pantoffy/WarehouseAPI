namespace WarehouseAPI.DTOs
{
    public static class WarehouseTypeMapping
    {
        // Dictionary<TypeId, (TypeName, Description)>
        private static readonly Dictionary<int, (string name, string description)> WarehouseTypeMap = new()
        {
            { 1, ("Vật tư", "Kho vật tư - Tiêu hao (Nguyên liệu, gia vị, bao bì, đồ gia dụng)") },
            { 2, ("Hàng hóa", "Kho hàng hóa - Bán lẻ (Nước, snack, mì, sản phẩm không chế biến)") },
            { 3, ("Tài sản", "Kho tài sản - Dài hạn (Máy móc, thiết bị, bàn ghế, không xuất nhập thường xuyên)") }
        };

        public static string GetWarehouseTypeName(int typeId)
        {
            return WarehouseTypeMap.TryGetValue(typeId, out var type) ? type.name : "Unknown";
        }

        public static string GetWarehouseTypeDescription(int typeId)
        {
            return WarehouseTypeMap.TryGetValue(typeId, out var type) ? type.description : "Unknown";
        }

        public static Dictionary<int, string> GetAllWarehouseTypes()
        {
            return WarehouseTypeMap.ToDictionary(x => x.Key, x => x.Value.name);
        }
    }
}
