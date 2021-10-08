using MusicPlatform.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public interface IUser 
    {
        Task<UserModel> GetUser(string username);
        Task<bool> IsVerified(string username);
        Task<string> GetUserId(string username);

    }
}
