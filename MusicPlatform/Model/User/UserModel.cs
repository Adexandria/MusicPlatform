using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace MusicPlatform.Model.User
{
    public class UserModel : IdentityUser
    {
        [Key]
        public override string Id { get => base.Id; set => base.Id = value; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string UserName { get => base.UserName; set => base.UserName = value; }
        public override string PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }
        public override string Email { get => base.Email; set => base.Email = value; }
        public override bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }
        public bool Verified { get; set; }
    }
}
