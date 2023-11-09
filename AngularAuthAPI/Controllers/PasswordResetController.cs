using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using AngularAuthAPI.Context;
using AngularAuthAPI.Models;

namespace AngularAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PasswordResetController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] PasswordResetRequest request)
        {
            try
            {
                // Find the user based on the provided ID
                var user = await _context.PasswordResetRequest.FirstOrDefaultAsync(u => u.ID == request.ID);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Reset the user's password to the new password
                user.Password = request.Password;

                // Save the changes to the database
                await _context.SaveChangesAsync();

                return Ok("Password reset successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
