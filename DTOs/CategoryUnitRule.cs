namespace WarehouseAPI.DTOs
{
    public static class CategoryUnitRule
    {
        public static readonly Dictionary<int, List<int>> Map = new()
        {
            { 1,  new() { 1, 2, 10, 11 } },        // Thịt: kg, g, con, quả
            { 2,  new() { 1, 2, 10, 11 } },         // Hải sản: kg, g, con, quả
            { 3,  new() { 1, 2, 20 } },              // Rau củ quả: kg, g, bó
            { 4,  new() { 2, 4, 13, 22 } },          // Gia vị: g, ml, gói, hũ/lọ
            { 5,  new() { 3, 4, 17, 23 } },          // Dầu mỡ: lít, ml, chai, can
            { 6,  new() { 1, 2, 13, 14 } },          // Lương thực: kg, g, gói, túi
            { 7,  new() { 3, 4, 11, 12, 17, 22 } },  // Sữa & Trứng: lít, ml, quả, hộp, chai, hũ/lọ
            { 8,  new() { 3, 4, 17, 18 } },           // Đồ uống: lít, ml, chai, lon
            { 9,  new() { 1, 2, 13, 14 } },           // Đồ khô: kg, g, gói, túi
            { 10, new() { 7, 12, 14, 15 } },          // Vật dụng: cái, hộp, túi, thùng
            { 11, new() { 3, 4, 17, 23 } },           // Vệ sinh: lít, ml, chai, can
            { 12, new() { 1, 2, 12, 16 } },           // Tráng miệng: kg, g, hộp, khay
            // Asset-only categories
            { 13, new() { 7, 8, 9 } },                // Nội thất: cái, chiếc, bộ
            { 14, new() { 7, 8, 9 } },                // Thiết bị: cái, chiếc, bộ
            { 15, new() { 7, 8, 9 } },                // Công cụ: cái, chiếc, bộ
            { 16, new() { 7, 8, 9 } }                 // Cơ sở vật chất: cái, chiếc, bộ
        };

        public static bool IsValidUnit(int categoryId, int unitId)
        {
            return Map.TryGetValue(categoryId, out var units) && units.Contains(unitId);
        }
    }
}
