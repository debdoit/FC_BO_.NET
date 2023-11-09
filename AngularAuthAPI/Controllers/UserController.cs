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
                Token= user.Token,
                Message = "Login Successful"
            });
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








            userobj.Password = PasswordHash.HashPassword(userobj.Password);
            userobj.Role = "User";
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
            var identity = new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),// Change to one hour expiration (adjust as needed)
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }


    }
}
