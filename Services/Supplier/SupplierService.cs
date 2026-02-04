using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
using WarehouseAPI.DTOs.SupplierDTOs;
using WarehouseAPI.Models;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Services.Supplier
{
    public class SupplierService : ISupplierService
    {
        private IUOW UOW;
        public SupplierService(IUOW uow)
        {
            UOW = uow;
        }
        public async Task<SupplierResponse> AddSupplierAsync(CreateSupplierRequest supplier)
        {
            return await UOW.SupplierRepository.AddSupplierAsync(supplier);
        }

        public async Task<bool> DeleteSupplierByIdAsync(int id)
        {
            return await UOW.SupplierRepository.DeleteSupplierByIdAsync(id);
        }

        public async Task<List<SupplierResponse>> GetAllSupplierAsync()
        {
            return await UOW.SupplierRepository.GetAllSupplierAsync();
        }


        public async Task<SupplierResponse> GetSupplierByIdAsync(int id)
        {
            return await UOW.SupplierRepository.GetSupplierByIdAsync(id);
        }

        public async Task<bool> UpdateSupplierByIdAsync(int id, UpdateSupplierRequest supplier)
        {
            return await UOW.SupplierRepository.UpdateSupplierByIdAsync(id, supplier);
        }
    }
}
