using WarehouseAPI.Data;
using WarehouseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace WarehouseAPI.Repository
{
    public class StockCheckTeamRepository
    {
        private readonly AppDbContext _context;

        public StockCheckTeamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StockCheckTeam?> GetByIdAsync(int id)
        {
            return await _context.StockCheckTeam
                .FirstOrDefaultAsync(sct => sct.Id == id);
        }

        public async Task<List<StockCheckTeam>> GetAllAsync()
        {
            return await _context.StockCheckTeam.ToListAsync();
        }

        public async Task<List<StockCheckTeam>> GetByStockCheckIdAsync(int stockCheckId)
        {
            return await _context.StockCheckTeam
                .Where(sct => sct.StockCheckId == stockCheckId)
                .ToListAsync();
        }

        public async Task<StockCheckTeam> CreateAsync(StockCheckTeam team)
        {
            _context.StockCheckTeam.Add(team);
            await _context.SaveChangesAsync();
            return team;
        }

        public async Task<StockCheckTeam> UpdateAsync(StockCheckTeam team)
        {
            _context.StockCheckTeam.Update(team);
            await _context.SaveChangesAsync();
            return team;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var team = await _context.StockCheckTeam.FindAsync(id);
            if (team == null) return false;

            _context.StockCheckTeam.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByStockCheckIdAsync(int stockCheckId)
        {
            var teams = await _context.StockCheckTeam
                .Where(sct => sct.StockCheckId == stockCheckId)
                .ToListAsync();

            if (teams.Count == 0) return false;

            _context.StockCheckTeam.RemoveRange(teams);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
