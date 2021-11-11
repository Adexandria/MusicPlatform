using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.Library;
using MusicPlatform.Model.Library.DTO;
using MusicPlatform.Services;
using System.Collections.Generic;


namespace MusicPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    public class TrendingController : ControllerBase
    {
        private readonly ISong _song;
        private readonly IMapper mapper;
        public TrendingController(ISong _song, IMapper mapper)
        {
            this._song = _song;
            this.mapper = mapper;
        }
        [HttpGet]
        public ActionResult<IEnumerable<SongsDTO>> GetTrendingSongs()
        {
            IEnumerable<SongModel> trendingSongs = _song.GetTrendingSong;
            IEnumerable<SongsDTO> mappedSongs = mapper.Map<IEnumerable<SongsDTO>>(trendingSongs);
            return Ok(mappedSongs);
        }
    }
}
