using MusicPlatform.Model.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public interface ILibrary
    {
        IEnumerable<UserLibrary> GetSongs { get; }
        Task<UserLibrary> GetSong(string name);
        Task<UserLibrary> GetSongById(Guid songId);

    }
}
