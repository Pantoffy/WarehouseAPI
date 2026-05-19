namespace WarehouseAPI.DTOs
{
    public readonly record struct UnitConversionRule(int FromUnitId, int ToUnitId, decimal Factor, int? MaterialId = null);
}
