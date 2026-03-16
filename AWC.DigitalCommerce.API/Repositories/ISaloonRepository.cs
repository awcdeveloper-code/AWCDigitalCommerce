using AWC.DigitalCommerce.API.Models.Domain;

namespace AWC.DigitalCommerce.API.Repositories
{
    public interface ISaloonRepository
    {
        Task <List<Seat>> GetAvailableSaloonAsync();

        Task<Seat?> GetSeatAsync(int id);
    }
}
