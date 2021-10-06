using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.User.Profile
{
    public class UserImage
    {
        [Key]
        public Guid ImageId { get; set; }
        [ForeignKey("Id")]
        public string UserId { get; set; }
        public string ImageUrl { get; set; }
        [ForeignKey("UserProfile")]
        public Guid ProfileId { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual UserModel User { get; set; }
       
    }
}
