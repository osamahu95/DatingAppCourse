using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MemberRepository(AppDbContext context) : IMemberRepository
    {
        private readonly AppDbContext context = context;

        public async Task<Member?> GetMemberByIdAsync(string Id)
        {
            return await context.Members.FindAsync(Id);
        }

        public async Task<IReadOnlyList<Member>> GetMembersAsync()
        {
            return await context.Members.ToListAsync();
        }

        public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
        {
            return await context.Members
                            .Where(x => x.Id == memberId)
                            .SelectMany(x => x.Photos)
                            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public void Update(Member member)
        {
            context.Entry(member).State = EntityState.Modified;
        }
    }
}
