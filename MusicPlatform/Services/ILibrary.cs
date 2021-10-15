using MusicPlatform.Model.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public interface ILibrary
    {
        IEnumerable<SongLibrary> GetLibrary(string username);
        IEnumerable<SongLibrary> GetSongLibrary(string username, string songName);
        Task AddToLibrary(string username,Guid songId);
        Task RemoveFromLibrary(string username, Guid songId);

    }
}
