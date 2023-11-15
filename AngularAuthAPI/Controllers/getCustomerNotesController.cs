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
                if (string.IsNullOrEmpty(customerId))
                {
                    return BadRequest("Customer ID is required.");
                }

                var upperCaseCustomerId = customerId.ToUpper(); // Convert to uppercase

                var customerNotes = await _context.CustomerNotes
                    .Where(note => note.CUSTOMER_ID.ToUpper() == upperCaseCustomerId)
                    .OrderBy(note => note.CREATED_DATE) // Order by CREATED_DATE
                    .Select(note => new
                    {
                        customeR_ID = note.CUSTOMER_ID,
                        notes = note.NOTES,
                        createD_DATE = note.CREATED_DATE ,// Format date
                        type = note.TYPE,
                        feedback = note.FEEDBACK,
                        updateD_BY = note.UPDATED_BY,
                        practioneR_ID = note.PRACTIONER_ID,
                        transactioN_ID = note.TRANSACTION_ID,
                    })
                    .ToListAsync();

                if (customerNotes.Any())
                {
                    return Ok(customerNotes);
                }
                else
                {
                    return NotFound("Customer notes not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }






        [HttpPost("AddCustomerNotes")]
        public async Task<IActionResult> AddCustomerNotes([FromBody] getCustomerNotes customerNotes, [FromQuery] string customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    return BadRequest("Customer ID is required.");
                }

                // Set the CUSTOMER_ID property using the value from the query parameter
                customerNotes.CUSTOMER_ID = customerId.ToUpper();

                // Set the CREATED_DATE property to the current date and time
                customerNotes.CREATED_DATE = DateTime.Now;

                // Insert the provided customerNotes into the TBL_CUSTOMER_NOTES table
                _context.CustomerNotes.Add(customerNotes);
                await _context.SaveChangesAsync();

                return Ok("Customer notes inserted successfully.");
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details for debugging purposes
                Console.WriteLine($"DbUpdateException: {ex.Message}");

                // Check if there is an inner exception with more details
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                return StatusCode(500, "Error saving changes to the database.");
            }

            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Exception: {ex}");

                // Log the stack trace if available
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }


    }
}
