using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AWC.DigitalCommerce.API.Models.Domain;
using AWC.DigitalCommerce.API.Models.DTO;
using AWC.DigitalCommerce.API.Repositories;

namespace AWC.DigitalCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaloonController : ControllerBase
    {
        private readonly ILogger<SaloonController> _logger;
        private readonly ISaloonRepository _saloon;

        public SaloonController(ISaloonRepository saloon, ILogger<SaloonController> logger)
        {
            _saloon = saloon;
            _logger = logger;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableSaloon()
        {
            try
            {
                List<Seat> availableSeats = await _saloon.GetAvailableSaloonAsync();

                _logger.LogInformation("GetAvailableSaloon executed succesfuly.");

                return Ok(availableSeats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAvailableSaloon Error: {ex.Message}{ex.StackTrace}");

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("seat")]
        public async Task<IActionResult> GetSeatAsync(int id)
        {
            try
            {
                Seat? seat = await _saloon.GetSeatAsync(id);

                _logger.LogInformation("GetSeatAsync executed succesfuly.");

                return Ok(seat);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetSeatAsync Error: {ex.Message}{ex.StackTrace}");

                return BadRequest(ex.Message);
            }
        }
    }
}
