using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularAuthAPI.Context;
using AngularAuthAPI.Models;

namespace AngularAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GDPRCustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GDPRCustomerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerGDPRDetails(string customerId)
        {
            try
            {
                var customer = await _context.GDPRCustomers.FirstOrDefaultAsync(c => c.CUSTOMER_ID == customerId);

                if (customer == null)
                {
                    return NotFound("Customer not found.");
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateGDPRCustomer/{customerId}")]
        public IActionResult UpdateGDPRCustomer(string customerId, [FromBody] GDPRCustomerUpdateRequest request)
        {
            var customer = _context.GDPRCustomers.FirstOrDefault(c => c.CUSTOMER_ID == customerId);
            if (customer == null)
            {
                // Return NotFound if the customer is not found
                return NotFound($"Customer with ID {customerId} not found.");
            }

            switch (request.UpdateType.ToLower())
            {
                case "display":
                    customer.DISPLAY = request.UpdateValue == 1;
                    break;
                case "hold":
                    customer.HOLD = request.UpdateValue == 1;
                    break;
                case "delete":
                    customer.DELETE = request.UpdateValue == 1;
                    break;
                default:
                    return BadRequest("Invalid UpdateType.");
            }

            _context.SaveChanges();

            return Ok("GDPR data updated successfully.");
        }

    }
}
