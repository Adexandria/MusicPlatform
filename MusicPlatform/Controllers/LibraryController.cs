using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.Library;
using MusicPlatform.Model.Library.DTO;
using MusicPlatform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Controllers
{
    [Route("api/{username}/[controller]")]
    [ApiController]
    [Authorize("BasicAuthentication")]
    public class LibraryController : ControllerBase
    {
        private readonly IUser userDetail;
        private readonly ISong _song;
        private readonly IMapper mapper;
        private readonly ILibrary _library;
        public LibraryController(ILibrary _library, IMapper mapper, IUser userDetail,ISong _song)
        {
            this.userDetail = userDetail;
            this.mapper = mapper;
            this._library = _library;
            this._song = _song;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserLibrariesDTO>> GetUserLibrary(string username)
        {
            var user = userDetail.GetUser(username).Result;
            if (user == null)
            {
                return NotFound("user not found");
            }
            var currentLibrary = _library.GetLibrary(username);
            var mappedLibrary = mapper.Map<IEnumerable<UserLibrariesDTO>>(currentLibrary);
            return Ok(mappedLibrary);
        }
        [HttpGet("songs")]
        public ActionResult<IEnumerable<UserLibraryDTO>> SearchSongs(string username,string songName)
        {
            var user = userDetail.GetUser(username).Result;
            if (user == null)
            {
                return NotFound("user not found");
            }
            var currentSongs = _library.GetSongLibrary(username, songName);
            if(currentSongs == null)
            {
                return NotFound();
            }
            var mappedLibrary = mapper.Map<IEnumerable<UserLibraryDTO>>(currentSongs);
            return Ok(mappedLibrary); 
        }
        [HttpPost("{songName}")]
        public async Task<IActionResult> AddLibrary(string username,string songName)
        {
            var user = await userDetail.GetUser(username);
            if (user == null)
            {
                return NotFound("user not found");
            }
            var currentSong = await _song.GetSong(username, songName);
            if(currentSong == null)
            {
                return NotFound();
            }
            await _library.AddToLibrary(username, currentSong.SongId);
            return Ok("Success");
        }
        [HttpDelete("{songName}")]
        public async Task<IActionResult> RemoveFromLibrary(string username, string songName)
        {
            var user = await userDetail.GetUser(username);
            if (user == null)
            {
                return NotFound("user not found");
            }
            var currentSong = await _song.GetSong(username, songName);
            if (currentSong == null)
            {
                return NotFound();
            }
            await _library.RemoveFromLibrary(username, currentSong.SongId);
            return Ok("Success");
        }
    }
}
