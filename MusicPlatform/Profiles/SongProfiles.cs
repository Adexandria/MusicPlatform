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
            CreateMap<SongCreate, SongModel>();
            CreateMap<SongUpdate, SongModel>();
            CreateMap<SongModel, SongDTO>().ForMember(s => s.SongName, opt => opt.MapFrom(s => s.SongName))
                .ForMember(s => s.SongURL, opt => opt.MapFrom(s => s.SongUrl))
                .ForMember(s => s.Download, opt => opt.MapFrom(s => s.Download))
                .ForPath(s=>s.ImageUrl,opt=>opt.MapFrom(s=>s.SongImage.ImageUrl))
                .ForPath(s => s.Credit.Performer, opt => opt.MapFrom(s => s.CreditModel.Performer))
                .ForPath(s => s.Credit.Writer, opt => opt.MapFrom(s => s.CreditModel.Writer))
                .ForPath(s => s.Credit.RecordLabel, opt => opt.MapFrom(s => s.CreditModel.RecordLabel))
                .ForPath(s => s.Credit.Producer, opt => opt.MapFrom(s => s.CreditModel.Producer))
                .ForMember(s => s.ReleasedDate, opt => opt.MapFrom(s => s.ReleasedDate));

            CreateMap<CreditCreate, CreditModel>();
            CreateMap<CreditUpdate, CreditModel>();

            CreateMap<SongLibrary, UserLibraryDTO>().ForPath(s => s.Artist, opt => opt.MapFrom(s => s.User.UserName))
                .ForPath(s => s.Imageurl, opt => opt.MapFrom(s => s.Song.SongImage.ImageUrl))
                .ForPath(s => s.SongUrl, opt => opt.MapFrom(s => s.Song.SongUrl))
                .ForPath(s => s.SongName, opt => opt.MapFrom(s => s.Song.SongName));

            CreateMap<SongLibrary, UserLibrariesDTO>().ForPath(s => s.Artist, opt => opt.MapFrom(s => s.User.UserName))
              .ForPath(s => s.SongName, opt => opt.MapFrom(s => s.Song.SongName));
        }
    }
}
