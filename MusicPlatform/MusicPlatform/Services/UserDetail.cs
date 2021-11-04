using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicPlatform.Model.User;
using MusicPlatform.Model.User.Profile;
using System;
using System.Collections.Generic;
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

        public IEnumerable<UserModel> GetUsers
        {
            get
            {
                var currentUsers = db.UserModel.Where(s => s.Verified == false).AsNoTracking();
                if (currentUsers != null)
                {
                    db.ChangeTracker.Clear();
                }
                return currentUsers;
            }
          

        }

        public IEnumerable<UserModel> GetArtists
        {
            get
            {
                var currentArtists = db.UserModel.Where(s => s.Verified == true).AsNoTracking();
                if (currentArtists != null)
                {
                    db.ChangeTracker.Clear();
                }
                return currentArtists;
            }
           
        }

        public IEnumerable<UserModel> SearchUser(string username)
        {
            var currentUser =  db.UserModel.Where(s => s.UserName.StartsWith(username)).Where(s => s.Verified == false).AsNoTracking();
            if (currentUser != null)
            {
                db.ChangeTracker.Clear();
            }
            return currentUser;
        }

        public  IEnumerable<UserModel> SearchArtist(string artist)
        {
            var currentartist = db.UserModel.Where(s => s.UserName.StartsWith(artist)).Where(s => s.Verified == true).AsNoTracking();
            if (currentartist != null)
            {
                db.ChangeTracker.Clear();
            }
            return currentartist;
        }
    }
}
