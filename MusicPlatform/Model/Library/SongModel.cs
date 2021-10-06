using MusicPlatform.Model.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.Library
{
    public class SongModel
    {
        [Key]
        public Guid SongId { get; set; }
        public string SongName { get; set; }
        [ForeignKey("Id")]
        public string ArtistId { get; set; }
        public DateTime ReleasedDate { get; set; } = DateTime.Now;
        public int Download { get; set; }

        public virtual CreditModel CreditModel { get; set; }
        public virtual UserModel User { get; set; }
        public virtual SongImage SongImage { get; set; }
    }
}
