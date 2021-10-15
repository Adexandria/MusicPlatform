using AutoMapper;
using MusicPlatform.Model.User;
using MusicPlatform.Model.User.Login;
using MusicPlatform.Model.User.Profile;
using MusicPlatform.Model.User.Profile.ProfileDTO;
using MusicPlatform.Model.User.SignUpDTO;
using System.Linq;

namespace MusicPlatform.Profiles
{
    public class UserProfiles : Profile
    {
        public UserProfiles()
        {
            CreateMap<SignUpUser, UserModel>().ForMember(s=>s.PasswordHash,opt=>opt.MapFrom(s=>s.Password));
            CreateMap<SignUpArtist, UserModel>().ForMember(s => s.PasswordHash, opt => opt.MapFrom(s => s.Password))
                .ForMember(s=>s.UserName,opt=>opt.MapFrom(s=>s.ArtistName));
            CreateMap<Login, UserModel>().ForMember(s => s.PasswordHash, opt => opt.MapFrom(s => s.Password));

            CreateMap<UserProfile, ArtistProfileDTO>().ForPath(s => s.Artistname, opt => opt.MapFrom(s => s.User.UserName))
               .ForMember(s => s.Followers, opt => opt.MapFrom(s => s.Followers.Count()))
                .ForMember(s => s.Following, opt => opt.MapFrom(s => s.Followings.Count()))
                .ForPath(s => s.Songs, opt => opt.MapFrom(s => s.Songs))
                .ForPath(s => s.ImageUrl, opt => opt.MapFrom(s => s.UserImage.ImageUrl));
                
            CreateMap<UserProfile, UserProfileDTO>().ForPath(s => s.Username, opt => opt.MapFrom(s => s.User.UserName))
                .ForMember(s => s.Following, opt => opt.MapFrom(s => s.Followings.Count()))
                .ForMember(s => s.Followers, opt => opt.MapFrom(s => s.Followers.Count()))
                .ForPath(s => s.ImageUrl, opt => opt.MapFrom(s => s.UserImage.ImageUrl));
            CreateMap<UserModel, UsersDTO>().ForMember(s => s.Username, opt => opt.MapFrom(s => s.UserName));
            CreateMap<UserModel, ArtistsDTO>().ForMember(s => s.Artist, opt => opt.MapFrom(s => s.UserName));
        }
    }
}
