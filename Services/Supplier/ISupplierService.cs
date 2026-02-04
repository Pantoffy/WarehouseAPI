using WarehouseAPI.DTOs.SupplierDTOs;
using WarehouseAPI.Models;
namespace WarehouseAPI.Services.Supplier
{
    public interface ISupplierService
    {
        Task<List<SupplierResponse>> GetAllSupplierAsync();
        Task<SupplierResponse> GetSupplierByIdAsync(int id);
        Task<SupplierResponse> AddSupplierAsync(CreateSupplierRequest supplier);
        Task<bool> UpdateSupplierByIdAsync(int id, UpdateSupplierRequest supplier);
        Task<bool> DeleteSupplierByIdAsync(int id);

    }
}
