using MusicPlatform.Model.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public interface ISong
    {
        Task AddSong(SongModel song,string artistId);
        Task<SongModel> GetSong(string artistId,Guid songId);
        IEnumerable<SongModel> GetSong(string name);
        Task UpdateSong(SongModel song,Guid songId,string artistId);
        Task DeleteSong(Guid songId, string artistId);
      //  IEnumerable<SongModel> GetSongs { get; }
       
       // Task<SongModel> GetSongById(Guid songId);
        Task AddCredit(CreditModel credit);
        Task UpdateCredit(CreditModel credit, Guid songId);
        Task DeleteCredit(Guid creditId);

        Task AddImage(string url,Guid songId);
        Task UpdateImage(SongImage image,Guid songId);
        Task DeleteImage(Guid songId);

        Task Save();

    }
}
