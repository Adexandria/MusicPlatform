using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.User.Profile
{
    public class FollowingModel
    {
        [Key]
        public Guid FollowingId { get; set; }
        [ForeignKey("AspNetUsersId")]
        public string UserId { get; set; }
        [ForeignKey("AspNetUsersId")]
        public string FollowerId { get; set; }
        [ForeignKey("UserProfile")]
        public Guid ProfileId { get; set; }
        public virtual UserProfile FollowingProfile { get; set; }
        [ForeignKey("UserProfile")]
        public Guid UserProfileProfileId { get; set; }
        [ForeignKey("ProfileId")]
        public virtual UserProfile FollowerProfile { get; set; }
       // public virtual UserModel User { get; set; }
    }
}
