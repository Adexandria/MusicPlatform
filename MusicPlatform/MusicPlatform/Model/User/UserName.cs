using System.ComponentModel.DataAnnotations;

namespace MusicPlatform.Model.User
{
    public class UserName
    {
        [Required]
        public string NewUsername { get; set; }
    }
}
