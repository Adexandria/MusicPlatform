using MusicPlatform.Model.User;
using MusicPlatform.Model.User.Profile;
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

        IEnumerable<UserModel> GetUsers { get; }
        IEnumerable<UserModel> GetArtists { get; }

        IEnumerable<UserModel> SearchUser(string username);
       IEnumerable<UserModel> SearchArtist(string artist);
    }
}
