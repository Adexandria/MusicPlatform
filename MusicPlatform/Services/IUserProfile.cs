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
        Task<UserProfile> GetUserProfile(string userId);
        Task<UserProfile> AddUserProfile(UserProfile userProfile);
        Task<UserProfile> UpdateUserProfile(UserProfile userProfile);
        Task<UserImage> AddImage(UserImage userImage);
        Task<UserImage> UpdateUserImage(UserImage userImage);
        Task DeleteImage(string userId);
        Task<FollowingModel> GetArtistFollowers(string artistId);
        Task<FollowingModel> GetUserFollowing(string userId);
        Task<FollowingModel> Follow(FollowingModel model);
        Task UnFollow(string artistId);
        Task<int> GetArtistSongs(string artistId);
        Task Save();

    }
}
