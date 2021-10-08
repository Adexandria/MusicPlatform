using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicPlatform.Model.Library;
using MusicPlatform.Model.User;
using MusicPlatform.Model.User.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Text_Speech.Services;

namespace MusicPlatform.Services
{
    public class UserProfileRepository : IUserProfile
    {
        private readonly DataDb db;
        private readonly IUser userDetail;
        private readonly IBlob blob;
        public UserProfileRepository(DataDb db, IUser userDetail, IBlob blob)
        {
            this.db = db ?? throw new NullReferenceException(nameof(db));
            this.userDetail = userDetail ?? throw new NullReferenceException(nameof(userDetail));
            this.blob = blob ?? throw new NullReferenceException(nameof(blob));
        }
        public async Task AddImage(string username, string imageUrl)
        {
            try
            {
                if (username == null)
                {
                    throw new NullReferenceException(nameof(username));
                }
                var currentimage = await GetImage(username);
                if (currentimage != null)
                {
                    await UpdateUserImage(imageUrl, username);
                }
                else
                {
                    var userId = await userDetail.GetUserId(username);
                    if (userId == null)
                    {
                        throw new NullReferenceException(nameof(userId));
                    }
                    var profileId = await GetUserProfileId(username);
                    UserImage userImage = new UserImage()
                    {
                        ImageId = Guid.NewGuid(),
                        UserId = userId,
                        ImageUrl = imageUrl,
                        ProfileId = profileId
                    };
                    await db.UserImages.AddAsync(userImage);
                    await Save();
                }

            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public async Task<UserImage> UpdateUserImage(string url, string username)
        {
            try
            {
                if (url == null)
                {
                    throw new NullReferenceException(nameof(url));
                }
                var currentImage = await GetImage(username);
                if (currentImage == null)
                {
                    throw new NullReferenceException(nameof(currentImage));
                }
                db.Entry(currentImage).State = EntityState.Detached;
                UserImage userImage = new UserImage()
                {
                    ImageUrl = url,
                    UserId = currentImage.UserId,
                    ImageId = currentImage.ImageId,
                    ProfileId = currentImage.ProfileId
                };
                db.Entry(userImage).State = EntityState.Modified;
                await blob.DeleteImage(currentImage.ImageUrl);
                await Save();
                return userImage;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public async Task DeleteImage(string username)
        {
            try
            {
                if (username == null)
                {
                    throw new NullReferenceException(nameof(username));
                }
                var currentimage = await GetImage(username);
                if (currentimage == null)
                {
                    throw new NullReferenceException(nameof(currentimage));
                }
                db.UserImages.Remove(currentimage);
                await blob.DeleteImage(currentimage.ImageUrl);
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }

        }


        //User Profile
        public async Task AddUserProfile(string username)
        {
            try
            {
                if (username == null)
                {
                    throw new NullReferenceException(nameof(username));
                }
                var userId = await userDetail.GetUserId(username);
                UserProfile userProfile = new UserProfile()
                {
                    ProfileId = Guid.NewGuid(),
                    UserId = userId
                };
                await db.Profiles.AddAsync(userProfile);
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public async Task<UserProfile> GetUserProfile(string username)
        {
            try
            {
                if (username == null)
                {
                    throw new NullReferenceException(nameof(username));
                }
                var userId = await userDetail.GetUserId(username);
                var currentprofile = await db.Profiles.Where(s => s.UserId == userId)
                    .Include(s => s.User).FirstOrDefaultAsync();
                if (await userDetail.IsVerified(username))
                {
                    currentprofile.Followers = GetFollowers(username).ToList();
                    currentprofile.Followings = GetFollowings(username).ToList();
                    currentprofile.Songs = GetArtistSongs(username).ToList();
                }
                else
                {
                    currentprofile.Followings = GetFollowings(username).ToList();
                    currentprofile.Followers = GetFollowers(username).ToList();
                }
                currentprofile.UserImage = await GetImage(username);
                return currentprofile;
            }
            catch (Exception e)
            {

                throw e;
            }
        }



        public async Task Follow(string username, string stagename)
        {
            try
            { 
                
                if (stagename == null)
                {
                    throw new NullReferenceException(nameof(stagename));
                }
                if (username == null)
                {
                    throw new NullReferenceException(nameof(username));
                }
                var userId = await userDetail.GetUserId(username);
                var artistId = await userDetail.GetUserId(stagename);
                var userProfile = await GetUserProfile(username);
                var artistProfile = await GetUserProfile(stagename);
                FollowingModel model = new FollowingModel()
                {
                    FollowingId = Guid.NewGuid(),
                    UserId = userId,
                    FollowerId = artistId,
                    UserProfileProfileId = artistProfile.ProfileId,
                    ProfileId = userProfile.ProfileId,
                   
                };
                db.ChangeTracker.Clear();
                db.Entry(model).State = EntityState.Added;
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public async Task UnFollow(string userId, string followerId)
        {
            try
            {
                if (userId == null)
                {
                    throw new NullReferenceException(nameof(userId));
                }
                if (followerId == null)
                {
                    throw new NullReferenceException(nameof(followerId));
                }
                var currentFollowing = await GetFollowing(userId, followerId);
                db.Followings.Remove(currentFollowing);
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public async Task<bool> IsFollowing (string userId,string followerId)
        {
            try
            {
                if (userId == null)
                {
                    throw new NullReferenceException(nameof(userId));
                }
                if (followerId == null)
                {
                    throw new NullReferenceException(nameof(followerId));
                }
                var currentFollowing = await GetFollowing(userId, followerId);
                if(currentFollowing == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {

                throw e;
            }
        }



        public async Task Save()
        {
            await db.SaveChangesAsync();
        }

        public async Task<UserImage> GetImage(string username)
        {
            var userId = await userDetail.GetUserId(username);
            if (userId == null)
            {
                throw new NullReferenceException(userId);
            }
            return await db.UserImages.Where(s => s.UserId == userId).AsNoTracking().FirstOrDefaultAsync();
        }

        private IEnumerable<SongModel> GetArtistSongs(string stagename)
        {
            var artistId = userDetail.GetUserId(stagename).Result;
            return db.Songs.Where(s => s.ArtistId == artistId).OrderByDescending(s => s.Download).Take(3).AsNoTracking();
        }
        private IEnumerable<FollowingModel> GetFollowers(string name)
        {
            if (name == null)
            {
                throw new NullReferenceException(nameof(name));
            }
            var artistId = userDetail.GetUserId(name).Result;
            return db.Followings.Where(s => s.FollowerId == artistId).AsNoTracking();
        }


        private IEnumerable<FollowingModel> GetFollowings(string name)
        {
            if (name == null)
            {
                throw new NullReferenceException(nameof(name));
            }
            var userId = userDetail.GetUserId(name).Result;
            return db.Followings.Where(s => s.UserId == userId).AsNoTracking();
        }
        private async Task<FollowingModel> GetFollowing(string userId, string artistId)
        {
            var currentFollowing = await db.Followings.Where(s => s.UserId == userId).Where(s => s.FollowerId == artistId).AsNoTracking().FirstOrDefaultAsync();
            if(currentFollowing != null)
            {
                db.Entry(currentFollowing).State = EntityState.Detached;
            }
              return currentFollowing;
        }
        private async Task<Guid> GetUserProfileId(string username)
        {
            var userId = await userDetail.GetUserId(username);
            return await db.Profiles.Where(s => s.UserId == userId).AsNoTracking().Select(s => s.ProfileId).FirstOrDefaultAsync();
        }

        
    }
}
