using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using src.Models.Entities;

namespace src.Models
{
    public class OneDbContext : IdentityDbContext<UserOne, RoleOne, string>
    {
        public DbSet<ClaimOne> ClaimOnes { get; set; }
        public OneDbContext(DbContextOptions<OneDbContext> options):base(options)
        {

        }
    }
}
