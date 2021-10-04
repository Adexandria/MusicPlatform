using Microsoft.AspNetCore.Identity;
using MusicPlatform.Model.User;
using System;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public class UserDetail
    {
        private readonly UserManager<UserModel> userManager;
        public UserDetail(UserManager<UserModel> userManager)
        {
            this.userManager = userManager ?? throw new NullReferenceException(nameof(userManager));
        }
        public async Task<string> GetUserId(string username)
        {
           UserModel current = await userManager.FindByNameAsync(username);
            return current.Id;
        }
        public async Task<bool> isVerified(string username)
        {
            UserModel current = await userManager.FindByNameAsync(username);
            return current.Verified;
        }
    }
}
