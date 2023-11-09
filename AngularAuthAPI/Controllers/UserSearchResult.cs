using Microsoft.AspNetCore.Mvc;
using AngularAuthAPI.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AngularAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSearchController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserSearchController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return BadRequest("Invalid search query.");
            }

            try
            {
                var results = await _context.UserSearchResults.FromSqlRaw(@"
                    SELECT ID, EMAIL, NAME AS ROLENAME, FIRSTNAME, LASTNAME
                    FROM (
                        SELECT U.ID, U.EMAIL, R.NAME, U.FIRSTNAME, U.LASTNAME,
                            ROW_NUMBER() OVER(PARTITION BY U.ID ORDER BY U.EMAIL) AS ROWRANK
                        FROM [DBO].[ASPNETUSERROLES] UR
                        JOIN [DBO].[ASPNETROLES] R ON R.ID = UR.ROLEID
                        JOIN [DBO].[ASPNETUSERS] U ON U.ID = UR.USERID
                        WHERE U.Email LIKE {0}
                            OR U.FirstName LIKE {0}
                            OR U.LastName LIKE {0}
                    ) RANKEDRESULTS
                    WHERE ROWRANK = 1", $"%{searchQuery}%").ToListAsync();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("customer-details")]
        public async Task<IActionResult> GetCustomerDetails(Guid id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    return NotFound("Customer not found.");
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
