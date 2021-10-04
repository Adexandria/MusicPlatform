using MusicPlatform.Model.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicPlatform.Model.User.Profile
{
    public class UserProfile
    {
        public string UserId { get; set; }
        public UserModel User { get; set; }
        public UserImage UserImage { get; set; }
        public List<FollowingModel> Following { get; set; }
        public List<SongModel> Song { get; set; }

    }
}
