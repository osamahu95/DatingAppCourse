using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController(AppDbContext context) : BaseApiController
    {
        private readonly AppDbContext context = context;

        [HttpPost("register")] // api/account/register
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await EmailExists(registerDto.Email)) return BadRequest("Email is already in use");
            
            var hmac = new HMACSHA512();

            var user = new AppUser 
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PaswordSalt = hmac.Key,
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        #region HelperFunctions
        private async Task<bool> EmailExists(string email)
        {
            return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }
        #endregion
    }
}
