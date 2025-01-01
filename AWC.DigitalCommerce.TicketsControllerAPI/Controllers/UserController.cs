using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AWC.DigitalCommerce.TicketsControllerAPI.Classes;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace AWC.DigitalCommerce.TicketsControllerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly APIContext _context;

        public UserController(APIContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetUsersList")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersList()
        {
            var usersList = await _context.Users.FromSqlRaw("EXEC GetUsersList")
                                                .AsNoTracking()
                                                .ToListAsync();
            return Ok(usersList);
        }

        [HttpGet(Name = "GetUserProfile")]
        public async Task<ActionResult<User>> GetUserProfile(int PIN)
        {
            var userProfile = await _context.Users.FromSqlRaw("EXEC GetUserProfile @PIN = {0}", PIN)
                                                  .AsNoTracking()
                                                  .ToListAsync();

            var user = userProfile.FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpDelete(Name = "DelUserProfile")]
        public async Task<IActionResult> DelUserProfile(int PIN)
        {
            var result = await _context.Database.ExecuteSqlRawAsync("EXEC DelUserProfile @PIN = {0}", new SqlParameter("@PIN", PIN));

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut(Name = "UpdlUserProfile")]
        public async Task<IActionResult> UpdlUserProfile(int PIN, User user)
        {
            var result = await _context.Database.ExecuteSqlRawAsync("EXEC UpdlUserProfile @PIN = {0}", new SqlParameter("@PIN", PIN));

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
