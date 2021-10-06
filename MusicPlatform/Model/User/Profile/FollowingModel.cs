using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.User.Profile
{
    public class FollowingModel
    {
        [Key]
        public Guid FollowingId { get; set; }
        [ForeignKey("Id")]
        public string UserId { get; set; }
        [ForeignKey("Id")]
        public string ArtistId { get; set; }
        [ForeignKey("UserProfile")]
        public Guid ProfileId { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public UserModel User { get; set; }
    }
}
