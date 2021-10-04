
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.User.Profile
{
    public class UserImage
    {
 
        [ForeignKey("User")]
        public string UserId { get; set; }
        public string ImageUrl { get; set; }
    }
}
