using WarehouseAPI.Data;
using WarehouseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace WarehouseAPI.Repository
{
    public class StockCheckRepository
    {
        private readonly AppDbContext _context;

        public StockCheckRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StockCheck?> GetByIdAsync(int id)
        {
            return await _context.StockCheck
                .Include(sc => sc.Warehouse)
                .Include(sc => sc.Teams)
                .FirstOrDefaultAsync(sc => sc.Id == id);
        }

        public async Task<List<StockCheck>> GetAllAsync()
        {
            return await _context.StockCheck
                .Include(sc => sc.Warehouse)
                .Include(sc => sc.Teams)
                .ToListAsync();
        }

        public async Task<List<StockCheck>> GetByWarehouseIdAsync(int warehouseId)
        {
            return await _context.StockCheck
                .Where(sc => sc.WarehouseId == warehouseId)
                .Include(sc => sc.Warehouse)
                .Include(sc => sc.Teams)
                .ToListAsync();
        }

        public async Task<StockCheck> CreateAsync(StockCheck stockCheck)
        {
            _context.StockCheck.Add(stockCheck);
            await _context.SaveChangesAsync();
            return stockCheck;
        }

        public async Task<StockCheck> UpdateAsync(StockCheck stockCheck)
        {
            _context.Entry(stockCheck).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return stockCheck;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var stockCheck = await _context.StockCheck.FindAsync(id);
            if (stockCheck == null) return false;

            _context.StockCheck.Remove(stockCheck);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
