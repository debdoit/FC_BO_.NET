using AngularAuthAPI.Context;
using AngularAuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AngularAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class getCustomerNotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public getCustomerNotesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetCustomerNotes")]
        public async Task<IActionResult> GetCustomerNotes([FromQuery] string customerId)
        {
            try
            {
                var customerNotes = await _context.CustomerNotes
                    .Where(note => note.CUSTOMER_ID == customerId.ToUpper())
                    .ToListAsync();

                if (customerNotes == null || customerNotes.Count == 0)
                {
                    return NotFound("Customer notes not found.");
                }

                return Ok(customerNotes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("AddCustomerNotes")]

        public async Task<IActionResult> AddCustomerNotes([FromBody] getCustomerNotes customerNotes)
        {
            try
            {
                // Insert the provided customerNotes into the TBL_CUSTOMER_NOTES table
                _context.CustomerNotes.Add(customerNotes);
                await _context.SaveChangesAsync();

                return Ok("Customer notes inserted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
