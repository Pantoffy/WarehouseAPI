using System.Collections.Concurrent;

namespace WarehouseAPI.DTOs
{
    public static class UnitConversionHelper
    {
        private static readonly ConcurrentDictionary<(int? MaterialId, int FromUnitId, int ToUnitId), decimal> Rules = new();

        static UnitConversionHelper()
        {
            RegisterRule(new UnitConversionRule(1, 2, 1000m)); // kg -> g
            RegisterRule(new UnitConversionRule(3, 4, 1000m)); // lít -> ml
            RegisterRule(new UnitConversionRule(5, 6, 100m)); // mét -> cm
        }

        public static void RegisterRule(UnitConversionRule rule)
        {
            if (rule.Factor <= 0)
                throw new ArgumentOutOfRangeException(nameof(rule.Factor));

            Rules[(rule.MaterialId, rule.FromUnitId, rule.ToUnitId)] = rule.Factor;
            Rules[(rule.MaterialId, rule.ToUnitId, rule.FromUnitId)] = 1m / rule.Factor;
        }

        public static decimal Convert(decimal quantity, int fromUnitId, int toUnitId, int? materialId = null)
        {
            if (fromUnitId == toUnitId)
                return quantity;

            if (TryGetFactor(materialId, fromUnitId, toUnitId, out var factor))
                return quantity * factor;

            throw new InvalidOperationException($"Không tìm thấy quy đổi từ đơn vị {fromUnitId} sang {toUnitId}.");
        }

        public static bool TryConvert(decimal quantity, int fromUnitId, int toUnitId, out decimal result, int? materialId = null)
        {
            if (fromUnitId == toUnitId)
            {
                result = quantity;
                return true;
            }

            if (TryGetFactor(materialId, fromUnitId, toUnitId, out var factor))
            {
                result = quantity * factor;
                return true;
            }

            result = default;
            return false;
        }

        public static bool HasConversion(int fromUnitId, int toUnitId, int? materialId = null)
        {
            return TryGetFactor(materialId, fromUnitId, toUnitId, out _);
        }

        private static bool TryGetFactor(int? materialId, int fromUnitId, int toUnitId, out decimal factor)
        {
            if (Rules.TryGetValue((materialId, fromUnitId, toUnitId), out factor))
                return true;

            if (materialId.HasValue && Rules.TryGetValue((null, fromUnitId, toUnitId), out factor))
                return true;

            if (UnitMapping.HasBaseUnit(fromUnitId) && UnitMapping.GetBaseUnitId(fromUnitId) == toUnitId)
            {
                factor = UnitMapping.GetFactorToBase(fromUnitId);
                return true;
            }

            if (UnitMapping.HasBaseUnit(toUnitId) && UnitMapping.GetBaseUnitId(toUnitId) == fromUnitId)
            {
                factor = 1m / UnitMapping.GetFactorToBase(toUnitId);
                return true;
            }

            if (UnitMapping.HasBaseUnit(fromUnitId) && UnitMapping.HasBaseUnit(toUnitId))
            {
                var fromBase = UnitMapping.GetBaseUnitId(fromUnitId);
                var toBase = UnitMapping.GetBaseUnitId(toUnitId);

                if (fromBase.HasValue && toBase.HasValue && fromBase == toBase)
                {
                    factor = UnitMapping.GetFactorToBase(fromUnitId) / UnitMapping.GetFactorToBase(toUnitId);
                    return true;
                }
            }

            factor = default;
            return false;
        }
    }
}
