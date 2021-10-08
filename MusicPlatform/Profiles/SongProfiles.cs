using AutoMapper;
using MusicPlatform.Model.Library;
using MusicPlatform.Model.Library.DTO;
using MusicPlatform.Model.User.Profile.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Profiles
{
    public class SongProfiles :Profile
    {
        public SongProfiles()
        {
            CreateMap<SongModel, SongsDTO>().ForMember(s => s.SongName, opt => opt.MapFrom(s => s.SongName));
            CreateMap<SongModel, SongDTO>().ForMember(s => s.SongName, opt => opt.MapFrom(s => s.SongName))
                .ForMember(s => s.SongURL, opt => opt.MapFrom(s => s.SongImage))
                .ForMember(s => s.Download, opt => opt.MapFrom(s => s.Download))
                .ForMember(s => s.Credit, opt => opt.MapFrom(s => s.CreditModel))
                .ForMember(s => s.ReleasedDate, opt => opt.MapFrom(s => s.ReleasedDate));
        }
    }
}
