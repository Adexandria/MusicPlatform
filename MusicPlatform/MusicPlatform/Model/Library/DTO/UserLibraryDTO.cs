using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Model.Library.DTO
{
    public class UserLibraryDTO
    {
      
        public string Artist { get; set; }
        public string SongName { get; set; }
        public string SongUrl { get; set; }
        public string Imageurl { get; set; }
    }
}
