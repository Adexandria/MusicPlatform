using Microsoft.EntityFrameworkCore;
using MusicPlatform.Model.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlatform.Model.User.Profile
{
    public class UserProfile
    {
        [Key]
        public Guid ProfileId { get; set; }
        [ForeignKey("Id")]
        public string UserId { get; set; }

        public virtual UserModel User { get; set; }
        public virtual UserImage UserImage { get; set; }
        public virtual List<FollowingModel> Following { get; set; }

    }
}
