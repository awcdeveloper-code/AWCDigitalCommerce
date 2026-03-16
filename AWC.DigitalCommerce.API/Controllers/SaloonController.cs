using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWC.DigitalCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaloonController : ControllerBase
    {
        private readonly ILogger<SaloonController> _logger;
        private readonly AppDbContext _dbContext;
        
        public SaloonController(AppDbContext dbContext, ILogger<SaloonController> _logger)
        {
            _dbContext = dbContext;

            _logger = _logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        public async Task<IActionResult> GetAvailableSaloon()
        {
            try
            {

                _logger.LogInformation("GetAvailableSaloon executed succesfuly.");

                return Ok("SaloonController");
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAvailableSaloon Error: {ex.Message}{ex.StackTrace}");
                return BadRequest(ex.Message);
            }
        }
    }
}
