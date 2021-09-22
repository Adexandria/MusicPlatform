using AutoMapper;
using MusicPlatform.Model.User;


namespace MusicPlatform.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<SignUpUser, UserModel>().ForMember(s=>s.PasswordHash,opt=>opt.MapFrom(s=>s.Password));
            CreateMap<SignUpArtist, UserModel>().ForMember(s => s.PasswordHash, opt => opt.MapFrom(s => s.Password))
                .ForMember(s=>s.UserName,opt=>opt.MapFrom(s=>s.ArtistName));
            CreateMap<Login, UserModel>().ForMember(s => s.PasswordHash, opt => opt.MapFrom(s => s.Password));
            
        }
    }
}
