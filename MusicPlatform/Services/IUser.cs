using MusicPlatform.Model.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public interface IUser 
    {
        Task<UserModel> GetUser(string username);
        Task AddUser(UserModel user);
        Task UpdateUser(string username,string user);
        Task<bool> IsVerified(string username);
        Task<string> GetUserId(string username);
        IEnumerable<UserModel> GetUsers { get; }
        IEnumerable<UserModel> GetArtists { get; }

        IEnumerable<UserModel> SearchUser(string username);
       IEnumerable<UserModel> SearchArtist(string artist);
    }
}
