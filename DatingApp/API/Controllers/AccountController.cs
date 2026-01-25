using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController(AppDbContext context) : BaseApiController
    {
        private readonly AppDbContext context = context;

        [HttpPost("register")] // api/account/register
        public async Task<ActionResult<AppUser>> Register(string email, string displayName, string password)
        {
            var hmac = new HMACSHA512();

            var user = new AppUser 
            {
                DisplayName = displayName,
                Email = email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PaswordSalt = hmac.Key,
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }
    }
}
