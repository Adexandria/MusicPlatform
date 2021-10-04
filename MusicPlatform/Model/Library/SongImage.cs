using MusicPlatform.Model.User.Profile;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.Library
{
    public class SongImage 
    {
        public string ImageUrl { get; set; }
        [ForeignKey("SongModel")]
        public Guid SongId { get; set; }
    }
}
