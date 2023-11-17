using AngularAuthAPI.Context;
using AngularAuthAPI.Helpers;
using AngularAuthAPI.Models;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace AngularAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;

        public UserController(AppDbContext appDbContext)
        {
            _authContext = appDbContext;
        }



        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userobj)
        {
            if (userobj == null)
                return BadRequest("Invalid request");

            var user = await _authContext.Users.FirstOrDefaultAsync(x => x.Username == userobj.Username);
            if (user == null)
                return NotFound("User Not Found");

            if (!PasswordHash.VerifyPassword(userobj.Password, user.Password))
            {
                return BadRequest("Incorrect Password");
            }

            user.Token = CreateJwt(user);

            return Ok(new
            {
                Token = user.Token,
                Message = "Login Successful",
                User = new
                {
                    Id = user.Id, // Replace with the actual property name for user identifier
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                    // Include other user details as needed
                }
            });
        }




        [HttpPut("update-profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] User updatedUser)
        {
            try
            {
                // Get the current user from the database based on the provided ID
                var currentUser = await _authContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (currentUser == null)
                    return NotFound($"User with ID {id} Not Found");

                // Update user properties
                currentUser.FirstName = updatedUser.FirstName;
                currentUser.LastName = updatedUser.LastName;
                currentUser.Email = updatedUser.Email;
                currentUser.Contact = updatedUser.Contact;

                // Save changes to the database
                _authContext.Entry(currentUser).State = EntityState.Modified;
                await _authContext.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Profile Updated Successfully",
                    User = new
                    {
                        Id = currentUser.Id,
                        Username = currentUser.Username,
                        FirstName = currentUser.FirstName,
                        LastName = currentUser.LastName,
                        Email = currentUser.Email,
                        Contact = currentUser.Contact
                    }
                });
            }
            catch (Exception ex)
            {
                // Handle exception (log or return an error response)
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }





        [HttpGet("get-user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _authContext.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                    return NotFound($"User with ID {id} Not Found");

                return Ok(new
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Contact = user.Contact,
                    Role = user.Role
                    // Include other user details as needed
                });
            }
            catch (Exception ex)
            {
                // Handle exception (log or return an error response)
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }



        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userobj)
        {
            if(userobj == null)
                return BadRequest();
            //Check username
            if(await CheckUserNameExistAsync(userobj.Username))
                return BadRequest(new {Message = "Username Already Exist!"});


            //Check Email
            if (await CheckEmailExistAsync(userobj.Email))
                return BadRequest(new { Message = "Email Already Exist!" });




            //Check password Strength
            var pass = CheckPasswordStrength(userobj.Password);
            if(!string.IsNullOrEmpty(pass))
                return BadRequest(new {Message = pass.ToString()});

            string userRole = userobj.Role;

            userobj.Password = PasswordHash.HashPassword(userobj.Password);
            userobj.Role = userRole;
            userobj.Token = "";
            await _authContext.Users.AddAsync(userobj);
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "User Registered!"
            });

        }

        private Task<bool> CheckUserNameExistAsync(string username) 
        => _authContext.Users.AnyAsync(x=>x.Username == username);
        private Task<bool> CheckEmailExistAsync(string email) 
        => _authContext.Users.AnyAsync(x=>x.Username == email);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if(password.Length < 8)
                sb.Append("Minimum password length should be 8 "+Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]")
                && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be Alphanumeric" + Environment.NewLine);
            if (!Regex.IsMatch(password, @"[!@#$%^&*()\-_+=\[\]{}|:;\""'<>,./?`~]"))
                sb.Append("Password should contain chars" + Environment.NewLine);
            return sb.ToString();
        }
        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");

            var identity = new ClaimsIdentity();

            // Add Id claim if not null
            if (user.Id > 0)
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            }

            // Add Role claim if not null
            if (!string.IsNullOrEmpty(user.Role))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
            }

            // Add Name claim if both first and last names are not null
            if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"));
            }

            // Add Username claim if not null
            if (!string.IsNullOrEmpty(user.Username))
            {
                identity.AddClaim(new Claim("Username", user.Username));
            }

            // Add FirstName claim if not null
            if (!string.IsNullOrEmpty(user.FirstName))
            {
                identity.AddClaim(new Claim("FirstName", user.FirstName));
            }

            // Add LastName claim if not null
            if (!string.IsNullOrEmpty(user.LastName))
            {
                identity.AddClaim(new Claim("LastName", user.LastName));
            }

            // Add Email claim if not null
            if (!string.IsNullOrEmpty(user.Email))
            {
                identity.AddClaim(new Claim("Email", user.Email));
            }

            // Add Contact_No claim if not null
            if (!string.IsNullOrEmpty(user.Contact))
            {
                identity.AddClaim(new Claim("Contact", user.Contact));
            }

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1), // Change to one hour expiration (adjust as needed)
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }






    }
}
