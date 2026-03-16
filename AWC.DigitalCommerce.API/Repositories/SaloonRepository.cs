using AWC.DigitalCommerce.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AWC.DigitalCommerce.API.Repositories
{
    public class SaloonRepository : ISaloonRepository
    {
        private readonly ILogger<SaloonRepository> _logger;
        private readonly AppDbContext _db;

        public SaloonRepository(AppDbContext db, ILogger<SaloonRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<Seat>> GetAvailableSaloonAsync()
        {
            try
            {
                _logger.LogInformation("GetAvailableSaloonAsync called.");

                return await _db.Seats.Where(s => s.Available).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAvailableSaloonAsync Error: {ex.Message}{ex.StackTrace}");
                return null!;
            }
        }

        public async Task<Seat?> GetSeatAsync(int id)
        {
            try
            {
                _logger.LogInformation("GetSeatAsync called.");

                return await _db.Seats.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetSeatAsync Error: {ex.Message}{ex.StackTrace}");
                return null!;
            }
        }
    }
}
