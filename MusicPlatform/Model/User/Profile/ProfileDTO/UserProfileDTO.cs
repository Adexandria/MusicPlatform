using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Model.User.Profile.ProfileDTO
{
    public class UserProfileDTO
    {
        public string Username { get; set; }
        public string ImageUrl { get; set; }
        public int Following { get; set; }
    }
}
