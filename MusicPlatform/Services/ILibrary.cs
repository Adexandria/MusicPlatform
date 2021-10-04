using MusicPlatform.Model.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public interface ILibrary
    {
        IEnumerable<SongModel> GetSongs { get; }
        IEnumerable<SongModel> GetSong(string name);
        Task<SongModel> GetSongById(Guid songId);

    }
}
