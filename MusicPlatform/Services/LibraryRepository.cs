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
        public LibraryRepository(DataDb db)
        {
            this.db = db ?? throw new NullReferenceException(nameof(db));
        }
        public IEnumerable<SongModel> GetSongs
        {
            get
            {
                return db.Songs.OrderBy(s => s.SongId);
            }
        }


        public IEnumerable<SongModel> GetSong(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new NullReferenceException(nameof(db));
            }
            return db.Songs.Where(s => s.SongName.StartsWith(name)).OrderBy(s => s.SongId).Include(s=>s.User)
                .Include(s=>s.CreditModel).Include(s=>s.SongImage);
        }

        public async Task<SongModel> GetSongById(Guid songId)
        {
            if(songId == null)
            {
                throw new NullReferenceException(nameof(db));
            }
            return await db.Songs.Where(s => s.SongId == songId).Include(s => s.User)
                .Include(s => s.CreditModel).Include(s => s.SongImage).FirstOrDefaultAsync();
        }
    }
}
