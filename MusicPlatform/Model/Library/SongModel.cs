using MusicPlatform.Model.User;
using MusicPlatform.Model.User.Profile;
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
        public string SongUrl { get; set; }
        [ForeignKey("Id")]
        public string UserId { get; set; }
        public DateTime ReleasedDate { get; set; } = DateTime.Now;
        public int Download { get; set; } = 0;
        [ForeignKey("UserProfile")]
        public Guid UserProfileProfileId { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual CreditModel CreditModel { get; set; }
        public virtual UserModel User { get; set; }
        public virtual SongImage SongImage { get; set; }


    }
}
