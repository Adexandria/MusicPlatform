using Microsoft.EntityFrameworkCore;
using MusicPlatform.Model.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public class LibraryRepository : ILibrary
    {
        private readonly DataDb db;
        private readonly IUser userDetail;
        public LibraryRepository(DataDb db, IUser userDetail)
        {
            this.db = db ?? throw new NullReferenceException(nameof(db));
            this.userDetail = userDetail ?? throw new NullReferenceException(nameof(userDetail));
        }

        public async Task AddToLibrary(string username,Guid songId)
        {
            if(songId == null)
            {
                throw new NullReferenceException(nameof(songId));
            }
            string userId = await userDetail.GetUserId(username);
            if (userId == null)
            {
                throw new NullReferenceException(nameof(userId));
            }
            SongLibrary library = new SongLibrary()
            {
                SongId = songId,
                LibraryId = Guid.NewGuid(),
                UserId = userId
            };
            await db.Libraries.AddAsync(library);
            await db.SaveChangesAsync();
        }

        public IEnumerable<SongLibrary> GetLibrary(string username)
        {
            if (username == null)
            {
                throw new NullReferenceException(nameof(username));
            }
            string userId =  userDetail.GetUserId(username).Result;
            if(userId == null)
            {
                throw new NullReferenceException(nameof(userId));
            }
            return  db.Libraries.Where(s => s.UserId == userId).Include(s => s.Song).Include(s=>s.User)
               .Include(s => s.Song.SongImage);
        }

       

        public async Task RemoveFromLibrary(string username, Guid songId)
        {
            if (songId == null)
            {
                throw new NullReferenceException(nameof(songId));
            }
            string userId = await userDetail.GetUserId(username);
            if (userId == null)
            {
                throw new NullReferenceException(nameof(userId));
            }
            SongLibrary song = await GetSongLibrary(userId,songId);
            if(song == null)
            {
                throw new NullReferenceException(nameof(song));
            }
            db.Libraries.Remove(song);
            await db.SaveChangesAsync();
        }

        public IEnumerable<SongLibrary> GetSongLibrary(string username, string songName)
        {
            if(string.IsNullOrWhiteSpace(songName))
            {
                throw new NullReferenceException(nameof(songName));
            }
            string userId = userDetail.GetUserId(username).Result;
            if (userId == null)
            {
                throw new NullReferenceException(nameof(userId));
            }
            return db.Libraries.Where(s => s.Song.SongName.StartsWith(songName)).Where(s=>s.UserId == userId).OrderBy(s => s.SongId).Include(s => s.Song)
                .Include(s => s.User).Include(a=>a.Song.SongImage);
        }
        private async Task<SongLibrary> GetSongLibrary(string username,Guid songId)
        {
            string userId = await userDetail.GetUserId(username);
            if (userId == null)
            {
                throw new NullReferenceException(nameof(userId));
            }
            return await db.Libraries.Where(s => s.SongId == songId).Where(s=>s.UserId == userId).FirstOrDefaultAsync();
        }
    }
}
