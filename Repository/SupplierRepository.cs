using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
using WarehouseAPI.DTOs.SupplierDTOs;
using WarehouseAPI.Models;

namespace WarehouseAPI.Repository
{
    public interface ISupplierRepository
    {
        Task<List<SupplierResponse>> GetAllSupplierAsync();
        Task<SupplierResponse?> GetSupplierByIdAsync(int id);
        Task<SupplierResponse> AddSupplierAsync(CreateSupplierRequest supplier);
        Task<bool> UpdateSupplierByIdAsync(int id, UpdateSupplierRequest supplier);
        Task<bool> DeleteSupplierByIdAsync(int id);

    }

    public class SupplierRepository(AppDbContext context) : ISupplierRepository
    {

        public async Task<SupplierResponse> AddSupplierAsync(CreateSupplierRequest supplier)
        {
            var newSupplier = new Supplier
            {
                Code = supplier.Code,
                Type = supplier.Type,
                Name = supplier.Name,
                ContactPerson = supplier.ContactPerson,
                Title = supplier.Title,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Role = supplier.Role,
                CitizenId = supplier.CitizenId,
                Address = supplier.Address,
                Status = supplier.Status,
                CreatedTime = supplier.CreatedTime
            };
            context.Suppliers.Add(newSupplier);
            await context.SaveChangesAsync();

            return new SupplierResponse
            {
                Id = newSupplier.Id,
                Code = newSupplier.Code,
                Type = newSupplier.Type,
                Name = newSupplier.Name,
                ContactPerson = newSupplier.ContactPerson,
                Title = newSupplier.Title,
                Phone = newSupplier.Phone,
                Email = newSupplier.Email,
                Role = newSupplier.Role,
                CitizenId = newSupplier.CitizenId,
                Address = newSupplier.Address,
                Status = newSupplier.Status,
                CreatedTime = newSupplier.CreatedTime
            };
        }

        public async Task<bool> DeleteSupplierByIdAsync(int id)
        {
            var supplier = await context.Suppliers.FindAsync(id);
            if (supplier is null)
                return false;

            context.Suppliers.Remove(supplier);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SupplierResponse>> GetAllSupplierAsync()
             => await context.Suppliers.Select(s => new SupplierResponse
             {
                 Id = s.Id,
                 Code = s.Code,
                 Type = s.Type,
                 Name = s.Name,
                 ContactPerson = s.ContactPerson,
                 Title = s.Title,
                 Phone = s.Phone,
                 Email = s.Email,
                 Role = s.Role,
                 CitizenId = s.CitizenId,
                 Address = s.Address,
                 Status = s.Status,
                 CreatedTime = s.CreatedTime
             }).ToListAsync();



        public async Task<SupplierResponse?> GetSupplierByIdAsync(int id)
        {
            var result = await context.Suppliers
                .Where(s => s.Id == id)
                .Select(s => new SupplierResponse
                {
                    Id = s.Id,
                    Code = s.Code,
                    Type = s.Type,
                    Name = s.Name,
                    ContactPerson = s.ContactPerson,
                    Title = s.Title,
                    Phone = s.Phone,
                    Email = s.Email,
                    Role = s.Role,
                    CitizenId = s.CitizenId,
                    Address = s.Address,
                    Status = s.Status,
                    CreatedTime = s.CreatedTime
                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> UpdateSupplierByIdAsync(int id, UpdateSupplierRequest supplier)
        {
            var existingSupplier = await context.Suppliers.FindAsync(id);
            if (existingSupplier is null) return false;

            existingSupplier.Code = supplier.Code;
            existingSupplier.Type = supplier.Type;
            existingSupplier.Name = supplier.Name;
            existingSupplier.ContactPerson = supplier.ContactPerson;
            existingSupplier.Title = supplier.Title;
            existingSupplier.Phone = supplier.Phone;
            existingSupplier.Email = supplier.Email;
            existingSupplier.Role = supplier.Role;  
            existingSupplier.CitizenId = supplier.CitizenId;
            existingSupplier.Address = supplier.Address;
            existingSupplier.Status = supplier.Status;
            existingSupplier.CreatedTime = supplier.CreatedTime;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
