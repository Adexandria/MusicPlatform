using System.ComponentModel.DataAnnotations;

namespace MusicPlatform.Model.User.SignUpDTO
{
    public class SignUpUser :SignUp
    {
        [Required(ErrorMessage = "Enter UserName")]
        public string UserName { get; set; }
    }
}
