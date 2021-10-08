using MusicPlatform.Model.User.Profile.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Model.User.Profile.ProfileDTO
{
    public class ArtistProfileDTO
    {
        public string Artistname { get; set; }
        public string ImageUrl { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        public List<SongDTO> Songs { get; set; }
    }
}
