using Microsoft.EntityFrameworkCore;
using MusicPlatform.Model.Library;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public class SongRepository : ISong
    {
        private readonly DataDb db;
        private readonly IUser userDetail;
        public SongRepository(DataDb db,IUser userDetail)
        {
            this.db = db ?? throw new NullReferenceException(nameof(db));
            this.userDetail = userDetail ?? throw new NullReferenceException(nameof(userDetail));
        }

        //Credit Repository
        public async Task AddCredit(CreditModel credit)
        {
            try
            {
                if (credit == null)
                {
                    throw new NullReferenceException(nameof(credit));
                }
                await db.SongCredits.AddAsync(credit);
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }
            

        }
        public async Task DeleteCredit(Guid songId)
        {
            try
            {
                if (songId == null)
                {
                    throw new NullReferenceException(nameof(songId));
                }
                var currentCredit = await GetCredit(songId);
                if (currentCredit == null)
                {
                    throw new NullReferenceException(nameof(currentCredit));
                }
                db.SongCredits.Remove(currentCredit);
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }
           
        }
        public async Task UpdateCredit(CreditModel credit, Guid songId)
        {
            try
            {
                if (songId == null)
                {
                    throw new NullReferenceException(nameof(songId));
                }
                if (credit == null)
                {
                    throw new NullReferenceException(nameof(credit));
                }
                var currentcredit = await GetCredit(songId);
                if (currentcredit == null)
                {
                    throw new NullReferenceException(nameof(currentcredit));
                }
                db.Entry(currentcredit).State = EntityState.Detached;
                credit.SongId = songId;
                db.Entry(credit).State = EntityState.Modified;
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }
            
        }

        //Image Repository

        public async Task AddImage(string url,Guid songId)
        {
            try
            {
                if (url == null)
                {
                    throw new NullReferenceException(nameof(url));
                }
                if (songId == null)
                {
                    throw new NullReferenceException(nameof(songId));
                }
                SongImage image = new SongImage()
                {
                    SongId = songId,
                    ImageUrl = url
                };
                await db.SongImages.AddAsync(image);
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }
           

        }
        public async Task UpdateImage(SongImage image, Guid songId)
        {
            try
            {
                if (songId == null)
                {
                    throw new NullReferenceException(nameof(songId));
                }
                if (image == null)
                {
                    throw new NullReferenceException(nameof(image));
                }
                var currentImage = await GetSongImage(songId);
                if (currentImage == null)
                {
                    throw new NullReferenceException(nameof(currentImage));
                }
                db.Entry(currentImage).State = EntityState.Detached;
                image.SongId = songId;
                db.Entry(image).State = EntityState.Modified;
            }
            catch (Exception e)
            {

                throw e;
            }
          
        }
        
        public async Task DeleteImage(Guid songId)
        {
            try
            {
                if (songId == null)
                {
                    throw new NullReferenceException(nameof(songId));
                }
                var currentImage = await GetSongImage(songId);
                if (currentImage == null)
                {
                    throw new NullReferenceException(nameof(currentImage));
                }
                db.SongImages.Remove(currentImage);
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }
         
        }



        //Song Repository
        public async Task AddSong(SongModel song, string username)
        {
            try
            {
                if (song == null)
                {
                    throw new NullReferenceException(nameof(song));
                }
                song.SongId = new Guid();
                var artistId = await userDetail.GetUserId(username);
                song.ArtistId = artistId;
                await db.Songs.AddAsync(song);
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }
          
        }
        public async Task DeleteSong(Guid songId,string artistId)
        {
            try
            {
                if (songId == null)
                {
                    throw new NullReferenceException(nameof(songId));
                }
                var currentSong = await GetSong(artistId, songId);
                if (currentSong == null)
                {
                    throw new NullReferenceException(nameof(currentSong));
                }
                db.Songs.Remove(currentSong);
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }
           
        }

        public async Task<SongModel> GetSong(string artistId,Guid songId)
        {
            try
            {
                if (songId == null)
                {
                    throw new NullReferenceException(nameof(songId));
                }
                if (artistId == null)
                {
                    throw new NullReferenceException(nameof(artistId));
                }
                var currentSong = await db.Songs.Where(a => a.SongId == songId).Where(s => s.ArtistId == artistId).Include(s => s.User).FirstOrDefaultAsync();
                currentSong.CreditModel = await GetCredit(songId);
                currentSong.SongImage = await GetSongImage(songId);
                return currentSong;
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

        public async Task UpdateSong(SongModel song, Guid songId,string artistId)
        {
            try
            {
                if (songId == null)
                {
                    throw new NullReferenceException(nameof(songId));
                }
                if (song == null)
                {
                    throw new NullReferenceException(nameof(song));
                }
                var currentSong = await GetSong(artistId, songId);
                if (currentSong == null)
                {
                    throw new NullReferenceException(nameof(currentSong));
                }
                db.Entry(currentSong).State = EntityState.Detached;
                song.ArtistId = artistId;
                song.SongId = songId;
                db.Entry(song).State = EntityState.Modified;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private async Task<CreditModel> GetCredit(Guid songId)
        {
            return await db.SongCredits.Where(s => s.SongId == songId).FirstOrDefaultAsync();
        }
        private async Task<SongImage> GetSongImage(Guid songId)
        {
            return await db.SongImages.Where(s => s.SongId == songId).FirstOrDefaultAsync();
        }
    }
}
