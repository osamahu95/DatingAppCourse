using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(AppDbContext context)
        {
            var membersData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var members = JsonSerializer.Deserialize<List<SeedUserDto>>(membersData);

            if (members == null || members.Count == 0)
            {
                Console.WriteLine("No Members in seed data");
                return;
            }


            foreach (var member in members)
            {
                var user = await context.Users
                    .Include(x => x.Member)
                    .SingleOrDefaultAsync(x => x.Id == member.Id);

                if (user == null)
                {
                    using var hmac = new HMACSHA512();
                    
                    var newMember = new Member
                    {
                        Id = member.Id,
                        DisplayName = member.DisplayName,
                        Description = member.Description,
                        DateOfBirth = member.DateOfBirth,
                        ImageUrl = member.ImageUrl,
                        Gender = member.Gender,
                        City = member.City,
                        Country = member.Country,
                        LastActive = member.LastActive,
                        Created = member.Created,
                    };

                    SetSinglePhotoInCollection(newMember);

                    user = new AppUser
                    {
                        Id = member.Id,
                        Email = member.Email,
                        DisplayName = member.DisplayName,
                        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                        PasswordSalt = hmac.Key,
                        Member = newMember
                    };
                    context.Users.Add(user);
                }

                if (user.Member == null)
                {
                    var newMember = new Member
                    {
                        Id = member.Id,
                        DisplayName = member.DisplayName,
                        Description = member.Description,
                        DateOfBirth = member.DateOfBirth,
                        ImageUrl = member.ImageUrl,
                        Gender = member.Gender,
                        City = member.City,
                        Country = member.Country,
                        LastActive = member.LastActive,
                        Created = member.Created,
                    };

                    SetSinglePhotoInCollection(newMember);
                    user.Member = newMember;
                    context.Users.Update(user);
                }
            }

            await context.SaveChangesAsync();
        }

        private static void SetSinglePhotoInCollection(Member member)
        {
            member.Photos.Add(new Photo
            {
                Url = member.ImageUrl!,
                MemberId = member.Id,
            });
        }
    }
}
