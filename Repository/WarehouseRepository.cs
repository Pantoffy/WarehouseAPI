using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
using WarehouseAPI.DTOs.WarehouseDTOs;
using WarehouseAPI.Models;

namespace WarehouseAPI.Repository
{
    public interface IWarehouseRepository
    {
        Task<List<WarehouseResponse>> GetAllWarehouseAsync();
        Task<WarehouseResponse?> GetWarehouseByIdAsync(int id);
        Task<WarehouseResponse> AddWarehouseAsync(CreateWarehouseRequest warehouse);
        Task<bool> UpdateWarehouseByIdAsync(int id, UpdateWarehouseRequest warehouse);
        Task<bool> DeleteWarehouseByIdAsync(int id);
    }

    public class WarehouseRepository(AppDbContext context) : IWarehouseRepository
    {
        public async Task<WarehouseResponse> AddWarehouseAsync(CreateWarehouseRequest warehouse)
        {
            var newWarehouse = new Warehouse
            {
                Code = warehouse.Code,
                Name = warehouse.Name,
                TypeId = warehouse.TypeId,
                Address = warehouse.Address,
                Area = warehouse.Area,
                ManagerName = warehouse.ManagerName,
                ManagerPhone = warehouse.ManagerPhone,
                Status = warehouse.Status,
                Note = warehouse.Note,
                CreatedTime = warehouse.CreatedTime
            };

            context.Warehouse.Add(newWarehouse);
            await context.SaveChangesAsync();

            return new WarehouseResponse
            {
                Id = newWarehouse.Id,
                Code = newWarehouse.Code,
                Name = newWarehouse.Name,
                TypeId = newWarehouse.TypeId,
                Address = newWarehouse.Address,
                Area = newWarehouse.Area,
                ManagerName = newWarehouse.ManagerName,
                ManagerPhone = newWarehouse.ManagerPhone,
                Status = newWarehouse.Status,
                Note = newWarehouse.Note,
                CreatedTime = newWarehouse.CreatedTime
            };
        }

        public async Task<bool> DeleteWarehouseByIdAsync(int id)
        {
            var warehouse = await context.Warehouse.FindAsync(id);
            if (warehouse is null)
                return false;

            context.Warehouse.Remove(warehouse);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<WarehouseResponse>> GetAllWarehouseAsync()
            => await context.Warehouse
                .Select(w => new WarehouseResponse
                {
                    Id = w.Id,
                    Code = w.Code,
                    Name = w.Name,
                    TypeId = w.TypeId,
                    Address = w.Address,
                    Area = w.Area,
                    ManagerName = w.ManagerName,
                    ManagerPhone = w.ManagerPhone,
                    Status = w.Status,
                    Note = w.Note,
                    CreatedTime = w.CreatedTime
                }).ToListAsync();

        public async Task<WarehouseResponse?> GetWarehouseByIdAsync(int id)
        {
            var result = await context.Warehouse
                .Where(w => w.Id == id)
                .Select(w => new WarehouseResponse
                {
                    Id = w.Id,
                    Code = w.Code,
                    Name = w.Name,
                    TypeId = w.TypeId,
                    Address = w.Address,
                    Area = w.Area,
                    ManagerName = w.ManagerName,
                    ManagerPhone = w.ManagerPhone,
                    Status = w.Status,
                    Note = w.Note,
                    CreatedTime = w.CreatedTime
                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> UpdateWarehouseByIdAsync(int id, UpdateWarehouseRequest warehouse)
        {
            var existingWarehouse = await context.Warehouse.FindAsync(id);
            if (existingWarehouse is null)
                return false;

            existingWarehouse.Code = warehouse.Code;
            existingWarehouse.Name = warehouse.Name;
            existingWarehouse.TypeId = warehouse.TypeId;
            existingWarehouse.Address = warehouse.Address;
            existingWarehouse.Area = warehouse.Area;
            existingWarehouse.ManagerName = warehouse.ManagerName;
            existingWarehouse.ManagerPhone = warehouse.ManagerPhone;
            existingWarehouse.Status = warehouse.Status;
            existingWarehouse.Note = warehouse.Note;
            existingWarehouse.CreatedTime = warehouse.CreatedTime;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
