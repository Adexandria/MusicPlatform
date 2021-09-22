using System.ComponentModel.DataAnnotations;


namespace MusicPlatform.Model.User
{
    public class SignUpArtist : SignUp
    {
        [Required(ErrorMessage = "Enter ArtistName")]
        public string ArtistName { get; set; }
    }
}
