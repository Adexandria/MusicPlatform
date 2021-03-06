using System.ComponentModel.DataAnnotations;


namespace MusicPlatform.Model.User.Login
{
    public class Login
    {
        [Required(ErrorMessage = "Enter UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }
    }
}
