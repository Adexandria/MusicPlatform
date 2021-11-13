using Microsoft.EntityFrameworkCore;
using MusicPlatform.Model.Library;
using MusicPlatform.Model.User.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Text_Speech.Services;

namespace MusicPlatform.Services
{
    public class SongRepository : ISong
    {
        private readonly DataDb db;
        private readonly IUser userDetail;
        private readonly IUserProfile _profile;
        private readonly IBlob blob;
        public SongRepository(DataDb db,IUser userDetail, IUserProfile _profile, IBlob blob)
        {
            this.db = db ?? throw new NullReferenceException(nameof(db));
            this.userDetail = userDetail ?? throw new NullReferenceException(nameof(userDetail));
            this._profile = _profile ?? throw new NullReferenceException(nameof(_profile));
            this.blob = blob ?? throw new NullReferenceException(nameof(blob));
        }

        //Credit Repository
        public async Task AddCredit(Guid songId,CreditModel credit)
        {
            try
            {
                if (credit == null)
                {
                    throw new NullReferenceException(nameof(credit));
                }
                credit.CreditId = Guid.NewGuid();
                credit.SongId = songId;
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
                CreditModel currentCredit = await GetCredit(songId);
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
                CreditModel currentcredit = await GetCredit(songId);
                if (currentcredit == null)
                {
                    throw new NullReferenceException(nameof(currentcredit));
                }
                db.Entry(currentcredit).State = EntityState.Detached;
                credit = UpdateCredit(currentcredit, credit);
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
                    SongImageid = Guid.NewGuid(),
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
        public async Task UpdateImage(string url, Guid songId)
        {
            try
            {
                if (songId == null)
                {
                    throw new NullReferenceException(nameof(songId));
                }
                if (url == null)
                {
                    throw new NullReferenceException(nameof(url));
                }
                var currentImage = await GetSongImage(songId);
                if (currentImage == null)
                {
                    throw new NullReferenceException(nameof(currentImage));
                }
                db.Entry(currentImage).State = EntityState.Detached;
                await blob.Delete(currentImage.ImageUrl);
                SongImage image = new SongImage()
                {
                    SongImageid = currentImage.SongImageid,
                    SongId = songId,
                    ImageUrl = url
                };
                db.Entry(image).State = EntityState.Modified;
                await Save();
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
                SongImage currentImage = await GetSongImage(songId);
                if (currentImage == null)
                {
                    throw new NullReferenceException(nameof(currentImage));
                }
                await blob.Delete(currentImage.ImageUrl);
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
                song.SongId = Guid.NewGuid();
                string artistId = await userDetail.GetUserId(username);
                UserProfile profile = await _profile.GetUserProfile(username);
                song.UserProfileProfileId = profile.ProfileId;
                song.UserId = artistId;
                await db.Songs.AddAsync(song);
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }
          
        }
        public async Task DeleteSong(string artistId,string songName)
        {
            try
            {
                if (songName == null)
                {
                    throw new NullReferenceException(nameof(songName));
                }
                SongModel currentSong = await GetSong(artistId, songName);
                if (currentSong == null)
                {
                    throw new NullReferenceException(nameof(currentSong));
                }
                await blob.Delete(currentSong.SongUrl);
                db.Songs.Remove(currentSong);
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }
           
        }

        public async Task<SongModel> GetSong(string username,string songName)
        {
            try
            {
                if (songName == null)
                {
                    throw new NullReferenceException(nameof(songName));
                }
                if (username == null)
                {
                    throw new NullReferenceException(nameof(username));
                }
                string artistId = await userDetail.GetUserId(username);
                var currentSong = await db.Songs.Where(a => a.SongName.ToLower() == songName.ToLower()).Where(s => s.UserId == artistId)
                    .Include(s => s.User).Include(s=>s.SongImage).Include(s=>s.CreditModel).Include(s=>s.SongImage).FirstOrDefaultAsync();
                return currentSong;
            }
            catch (Exception e)
            {

                throw e;
            }
           
        }
        public IEnumerable<SongModel> GetSong(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new NullReferenceException(nameof(db));
            }
            return db.Songs.Where(s => s.SongName.StartsWith(name)).OrderBy(s => s.SongId).Include(s => s.User)
                .Include(s => s.CreditModel).Include(s => s.SongImage);
        }

      
        public async Task Save()
        {
            await db.SaveChangesAsync();
        }

        public async Task UpdateSong(SongModel song, string songName, string artistId)
        {
            try
            {
                if (songName == null)
                {
                    throw new NullReferenceException(nameof(songName));
                }
                if (song == null)
                {
                    throw new NullReferenceException(nameof(song));
                }
                SongModel currentSong = await GetSong(artistId, songName);
                if (currentSong == null)
                {
                    throw new NullReferenceException(nameof(currentSong));
                }
                db.Entry(currentSong).State = EntityState.Detached;
                song.UserId = artistId;
                song.SongId = currentSong.SongId;
                await blob.Delete(currentSong.SongUrl);
                db.Entry(song).State = EntityState.Modified;
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        
        public async Task DownloadSong(string artist,string songName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(songName))
                {
                    throw new NullReferenceException(nameof(songName));
                }
                SongModel currentSong = await GetSong(artist, songName);
                if (currentSong == null)
                {
                    throw new NullReferenceException(nameof(currentSong));
                }
                db.Entry(currentSong).State = EntityState.Detached;
                SongModel model = new SongModel()
                {
                    SongId = currentSong.SongId,
                    UserId = currentSong.User.Id,
                    SongName = currentSong.SongName,
                    SongUrl = currentSong.SongUrl,
                    ReleasedDate = currentSong.ReleasedDate,
                    UserProfileProfileId = currentSong.UserProfileProfileId,
                    Download = currentSong.Download + 1
                };
                db.Entry(model).State = EntityState.Modified;
                await Save();
            }
            catch (Exception e)
            {

                throw e;
            }
           
        }
        public IEnumerable<SongModel> GetTrendingSong 
        {
            get
            {
                return db.Songs.OrderByDescending(s => s.Download).Take(5);
            }
        }

        public IEnumerable<SongModel> GetNewSongs
        {
            get
            {
                return db.Songs.OrderBy(s => s.ReleasedDate).Take(5);
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
        private CreditModel UpdateCredit(CreditModel credit,CreditModel updatedCredit)
        {
            updatedCredit.SongId = credit.SongId;
            updatedCredit.CreditId = credit.CreditId;
            if (string.IsNullOrEmpty(updatedCredit.Performer))
            {
                updatedCredit.Performer = credit.Performer;
            }
            if (string.IsNullOrEmpty(updatedCredit.Producer))
            {
                updatedCredit.Producer = credit.Producer;
            }
            if (string.IsNullOrEmpty(updatedCredit.Writer))
            {
                updatedCredit.Writer = credit.Writer;
            }
            if (string.IsNullOrEmpty(updatedCredit.RecordLabel))
            {
                updatedCredit.RecordLabel = credit.RecordLabel;
            }
            return updatedCredit;
        }
       
    }
}
