

using System.ComponentModel.DataAnnotations;

namespace MusicPlatform.Model.User
{
    public class ResetPassword
    {
        [Required(ErrorMessage ="Enter Token")]
        public string Token { get; set; }
        [Required(ErrorMessage ="Enter Password")]
        
        public string NewPassword { get; set; }
    }
}
