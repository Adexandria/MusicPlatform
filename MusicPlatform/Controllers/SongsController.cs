using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.Library.DTO;
using MusicPlatform.Model.User.Profile.DTO;
using MusicPlatform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Text_Speech.Services;

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
        private readonly IUser userDetail;
        public SongsController(ISong _song,IMapper mapper, IUser userDetail, IBlob _blob)
        {
            this._song = _song;
            this.mapper = mapper;
            this.userDetail = userDetail;
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
                var currentUser = await userDetail.GetUser(artist);
                if (currentUser == null)
                {
                    return NotFound("User not found");
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
                var currentUser = await userDetail.GetUser(artist);
                if (currentUser == null)
                {
                    return NotFound("User not found");
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
    }
}
