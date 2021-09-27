using MusicPlatform.Model.User.Profile;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.Library
{
    public class SongImage : UserImage
    {
        [ForeignKey("SongModel")]
        public Guid SongId { get; set; }
    }
}
