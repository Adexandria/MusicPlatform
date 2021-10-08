using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicPlatform.Model.User;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public class UserDetail : IUser
    {
        private readonly DataDb db;
        public UserDetail( DataDb db)
        {
            this.db = db ?? throw new NullReferenceException(nameof(db));
        }
        public async Task<string> GetUserId(string username)
        {
            var currentUser = await GetUser(username);
            return currentUser.Id;
        }
        public async Task<bool> IsVerified(string username)
        {
            var currentUser = await GetUser(username);
            return currentUser.Verified;
        }
        public async Task<UserModel> GetUser(string username)
        {
            var currentUser = await db.UserModel.Where(s => s.UserName == username).AsNoTracking().FirstOrDefaultAsync();
            if (currentUser != null)
            {
                db.Entry(currentUser).State = EntityState.Detached;
            }
            return currentUser;
        }
    }
}
