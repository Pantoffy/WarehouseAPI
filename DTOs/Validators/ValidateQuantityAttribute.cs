using System.ComponentModel.DataAnnotations;

namespace WarehouseAPI.DTOs.Validators
{
    public class ValidateQuantityAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not decimal quantity)
                return ValidationResult.Success!;

            var request = validationContext.ObjectInstance;
            var unitIdProperty = request.GetType().GetProperty("UnitId");

            if (unitIdProperty?.GetValue(request) is not int unitId)
                return ValidationResult.Success!;

            if (quantity <= 0)
                return new ValidationResult("Số lượng phải lớn hơn 0");

            // Kiểm tra nếu đơn vị không cho phép thập phân
            if (!UnitMapping.IsDecimalAllowed(unitId) && quantity != Math.Floor(quantity))
                return new ValidationResult($"Đơn vị '{UnitMapping.GetUnitName(unitId)}' chỉ được nhập số nguyên");

            return ValidationResult.Success!;
        }
    }
}
