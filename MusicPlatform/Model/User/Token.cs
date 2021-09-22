using System.ComponentModel.DataAnnotations;

namespace MusicPlatform.Model.User
{
    public class Token
    {  
        [Required]
        public string GeneratedToken { get; set; }
    }
}