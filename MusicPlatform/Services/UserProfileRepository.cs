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
        private readonly UserDetail userDetail;
        private readonly IBlob blob;
        public UserProfileRepository( DataDb db, UserDetail userDetail,IBlob blob)
        {
            this.db = db ?? throw new NullReferenceException(nameof(db));
            this.userDetail = userDetail ?? throw new NullReferenceException(nameof(userDetail));
            this.blob = blob ?? throw new NullReferenceException(nameof(blob));
        }
        public async Task AddImage(string username,string imageUrl)
        {
            try
            {
                if (username == null)
                {
                    throw new NullReferenceException(nameof(username));
                }
                var currentimage = await GetImage(username);
                if(currentimage != null)
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
                        ImageId = new Guid(),
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
                if(currentImage == null)
                {
                    throw new NullReferenceException(nameof(currentImage));
                }
                db.Entry(currentImage).State = EntityState.Detached;
                UserImage userImage = new UserImage() {
                    ImageUrl = url, 
                    UserId = currentImage.UserId,
                    ImageId = currentImage.ImageId,
                    ProfileId = currentImage.ProfileId
                };
                db.Entry(userImage).State = EntityState.Modified;
                await  blob.DeleteImage(currentImage.ImageUrl);
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
                if(currentimage == null)
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
                    ProfileId = new Guid(),
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
                var currentprofile = await db.Profiles.Where(s => s.UserId == userId).Include(s=>s.UserImage)
                    .FirstOrDefaultAsync();
                if (await userDetail.isVerified(username))
                {
                    currentprofile.Following = GetArtistFollowers(username).ToList();
                }
                else
                {
                    currentprofile.Following = GetUserFollowings(username).ToList();
                }
                currentprofile.User = await userDetail.GetUser(username);
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
                FollowingModel model = new FollowingModel()
                {
                    UserId = userId,
                    ArtistId = artistId,
                    FollowingId = new Guid()
                };
                await db.Followings.AddAsync(model);
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }
           
        }
        public async Task UnFollow(string username, string stagename)
        {
            try
            {
                if (username == null)
                {
                    throw new NullReferenceException(nameof(username));
                }
                if (stagename == null)
                {
                    throw new NullReferenceException(nameof(username));
                }
                var currentFollowing = await GetUserFollowing(username, stagename);
                db.Followings.Remove(currentFollowing);
                await Save();
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
            if(userId == null)
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
        private IEnumerable<FollowingModel> GetArtistFollowers(string stagename)
        {
            if (stagename == null)
            {
                throw new NullReferenceException(nameof(stagename));
            }
            var artistId = userDetail.GetUserId(stagename).Result;
            return db.Followings.Where(s => s.ArtistId == artistId).AsNoTracking();
        }


        private IEnumerable<FollowingModel> GetUserFollowings(string username)
        {
            if (username == null)
            {
                throw new NullReferenceException(nameof(username));
            }
            var userId = userDetail.GetUserId(username).Result;
            return db.Followings.Where(s => s.UserId == userId).AsNoTracking();
        }
        private async Task<FollowingModel> GetUserFollowing(string username,string stagename)
        {
            var userId = await userDetail.GetUserId(username);
            var artistId = await userDetail.GetUserId(stagename);
            return await db.Followings.Where(s => s.UserId == userId).Where(s=>s.ArtistId == artistId).AsNoTracking().FirstOrDefaultAsync();
        }
        private async Task<Guid> GetUserProfileId(string username)
        {
            var userId = await userDetail.GetUserId(username);
            return await db.Profiles.Where(s => s.UserId == userId).AsNoTracking().Select(s=>s.ProfileId).FirstOrDefaultAsync();
        }
    }
}
