using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MusicPlatform.Model.Library.DTO;
using MusicPlatform.Model.User.Profile.DTO;
using MusicPlatform.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Text_Speech.Services;
using Microsoft.AspNetCore.Identity;
using MusicPlatform.Model.User;

namespace MusicPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SongsController : ControllerBase
    {
        private readonly ISong _song;
        private readonly IBlob _blob;
        private readonly IMapper mapper;
        public SongsController(ISong _song,IMapper mapper, IBlob _blob)
        {
            this._song = _song;
            this.mapper = mapper;
            this._blob = _blob;
        }

        [HttpGet]
        public ActionResult<IEnumerable<SongsDTO>> GetSongs(string songName)
        {
            try
            {
                var songs = _song.GetSong(songName);
                var mappedSongs = mapper.Map<IEnumerable<SongsDTO>>(songs);
                return Ok(mappedSongs);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        [HttpGet("{artist}/{songName}")]
        public async Task<ActionResult<SongDTO>> GetSong(string artist,string songName)
        {
            try
            {
                if(Request.Cookies["Download"] == null)
                {
                    Response.Cookies.Append("Download", "4");
                }

                var song = await _song.GetSong(artist,songName);
                if(song == null)
                {
                    return NotFound("Song not found");
                }
                var mappedSongs = mapper.Map<SongDTO>(song);
                return Ok(mappedSongs);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        [HttpGet("{artist}/{songName}/Download")]
        public async Task<ActionResult<SongDTO>> DownloadSong(string artist, string songName)
        {
            try
            {
                var isSigned = IsSignedIn();
                if (!isSigned)
                {
                    return BadRequest("You reached your limit, sign in");
                }

                var song = await _song.GetSong(artist, songName);
                if (song == null)
                {
                    return NotFound("Song not found");
                }
                
                await  _song.DownloadSong(artist,songName);
                var file = await _blob.DownloadFile(song.SongUrl);
                return File(file.Value.Content, "audio/mpeg", songName);

            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        [NonAction]
        public bool IsFree()
        {
            var x = Request.Cookies["Download"];
            var y = Convert.ToInt32(x);
            if (y == 0)
            {
                return false;
            }
           var value = (y - 1).ToString();
           Response.Cookies.Append("Download", value);  
           return true;
        }

        [NonAction]
        public bool IsSignedIn()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return IsFree();
            }
            return true;
        }
    }
}
