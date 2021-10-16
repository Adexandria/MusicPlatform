using MusicPlatform.Model.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public interface ISong
    {
        Task AddSong(SongModel song,string artist);
        Task<SongModel> GetSong(string artistId,string songName);
        IEnumerable<SongModel> GetSong(string name);
        Task UpdateSong(SongModel song, string songName, string artistId);
        Task DeleteSong(string artistId,string songName);
      //  IEnumerable<SongModel> GetSongs { get; }
       
       // Task<SongModel> GetSongById(Guid songId);
        Task AddCredit(Guid songId,CreditModel credit);
        Task UpdateCredit(CreditModel credit, Guid songId);
        Task DeleteCredit(Guid songId);

        Task AddImage(string url,Guid songId);
        Task UpdateImage(string url, Guid songId);
        Task DeleteImage(Guid songId);

        Task DownloadSong(string artist,string songName);


        IEnumerable<SongModel> GetTrendingSong { get; }
        IEnumerable<SongModel> GetNewSongs { get; }
        Task Save();

    }
}
