using System.ComponentModel.DataAnnotations;

namespace MusicPlatform.Model.User
{
    public abstract class SignUp
    {
        [Required(ErrorMessage ="Enter FirstName")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Enter LastName")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Enter Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Enter RetypePassword")]
        public string RetypePassword { get; set; }
    }
}
