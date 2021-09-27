using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.User.Profile
{
    public class FollowingModel
    {
        [Key]
        public Guid FollowingId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public UserModel User { get; set; }
        public string ArtistId { get; set; }
    }
}
