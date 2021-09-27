using MusicPlatform.Model.User;
using System;
using System.ComponentModel.DataAnnotations;


namespace MusicPlatform.Model.Library
{
    public class SongModel
    {
        [Key]
        public Guid SongId { get; set; }
        public string SongName { get; set; }
        public SongImage SongImage { get; set; }
        public DateTime ReleasedDate { get; set; } = DateTime.Now;
        public CreditModel CreditModel { get; set; }
        public string ArtistId { get; set; }
        public UserModel User { get; set; }
        public int Download { get; set; }
    }
}
