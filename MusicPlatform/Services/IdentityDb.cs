using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MusicPlatform.Model.User;

namespace MusicPlatform.Services
{
    public class IdentityDb : IdentityDbContext<UserModel>
    {
        public IdentityDb(DbContextOptions<IdentityDb> options) : base(options)
        {

        }
        public DbSet<UserModel> User { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
