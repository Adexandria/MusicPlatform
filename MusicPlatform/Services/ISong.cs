using MusicPlatform.Model.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public interface ISong
    {
        Task<SongModel> AddSong(SongModel song);
        Task<SongModel> GetSong(string artistId);
        Task<SongModel> UpdateSong(SongModel song,Guid songId);
        Task DeleteSong(Guid songId);

        Task<CreditModel> AddCredit(CreditModel credit);
        Task<CreditModel> UpdateCredit(CreditModel credit, Guid songId);
        Task DeleteCredit(Guid creditId);

        Task<SongImage> AddImage(SongImage image);
        Task<SongImage> UpdateImage(SongImage image,Guid songId);
        Task DeleteImage(Guid songId);

    }
}
