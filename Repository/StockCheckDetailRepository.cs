using WarehouseAPI.Data;
using WarehouseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace WarehouseAPI.Repository
{
    public class StockCheckDetailRepository
    {
        private readonly AppDbContext _context;

        public StockCheckDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StockCheckDetail?> GetByIdAsync(int id)
        {
            return await _context.StockCheckDetail
                .Include(scd => scd.Material)
                .Include(scd => scd.Warehouse)
                .FirstOrDefaultAsync(scd => scd.Id == id);
        }

        public async Task<List<StockCheckDetail>> GetAllAsync()
        {
            return await _context.StockCheckDetail
                .Include(scd => scd.Material)
                .Include(scd => scd.Warehouse)
                .ToListAsync();
        }

        public async Task<List<StockCheckDetail>> GetByStockCheckIdAsync(int stockCheckId)
        {
            return await _context.StockCheckDetail
                .Where(scd => scd.StockCheckId == stockCheckId)
                .Include(scd => scd.Material)
                .Include(scd => scd.Warehouse)
                .ToListAsync();
        }

        public async Task<StockCheckDetail> CreateAsync(StockCheckDetail detail)
        {
            _context.StockCheckDetail.Add(detail);
            await _context.SaveChangesAsync();
            return detail;
        }

        public async Task<StockCheckDetail> UpdateAsync(StockCheckDetail detail)
        {
            _context.StockCheckDetail.Update(detail);
            await _context.SaveChangesAsync();
            return detail;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var detail = await _context.StockCheckDetail.FindAsync(id);
            if (detail == null) return false;

            _context.StockCheckDetail.Remove(detail);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByStockCheckIdAsync(int stockCheckId)
        {
            var details = await _context.StockCheckDetail
                .Where(scd => scd.StockCheckId == stockCheckId)
                .ToListAsync();

            if (details.Count == 0) return false;

            _context.StockCheckDetail.RemoveRange(details);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
