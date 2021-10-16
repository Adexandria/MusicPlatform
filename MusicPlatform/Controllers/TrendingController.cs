using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.Library.DTO;
using MusicPlatform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
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
            var trendingSongs = _song.GetTrendingSong;
            var mappedSongs = mapper.Map<IEnumerable<SongsDTO>>(trendingSongs);
            return Ok(mappedSongs);
        }
    }
}
