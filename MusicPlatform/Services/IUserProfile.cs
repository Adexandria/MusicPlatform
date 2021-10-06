using MusicPlatform.Model.Library;
using MusicPlatform.Model.User.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public interface IUserProfile
    {
        Task<UserProfile> GetUserProfile(string username);
        Task AddUserProfile(string username);
        Task AddImage(string username, string imageUrl);
        Task<UserImage> GetImage(string username);
        Task<UserImage> UpdateUserImage(string url,string username);
        Task DeleteImage(string username);
        //IEnumerable<FollowingModel> GetArtistFollowers(string stagename);
      //  IEnumerable<FollowingModel> GetUserFollowing(string username);
        Task Follow(string username,string stagename);
        Task UnFollow(string username,string stagename);
        Task Save();

    }
}
