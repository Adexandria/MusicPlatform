using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.Library;
using MusicPlatform.Model.Library.DTO;
using MusicPlatform.Model.User;
using MusicPlatform.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlatform.Controllers
{
    [Route("api/{username}/[controller]")]
    [ApiController]
    [Authorize("BasicAuthentication")]
    [ApiVersion("1.0")]
   // [ApiVersion("2.0")]
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

        ///<param name="username">
        ///an string object 
        ///</param>
        /// <summary>
        /// Get User Library
        /// </summary>
        /// 
        /// <returns>returns UserLibraries Objects </returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet]
        public ActionResult<IEnumerable<UserLibrariesDTO>> GetUserLibrary(string username)
        {

            try
            {
                UserModel user = userDetail.GetUser(username).Result;
                if (user == null)
                {
                    return NotFound("user not found");
                }
                IEnumerable<SongLibrary> currentLibrary = _library.GetLibrary(username);
                IEnumerable<UserLibrariesDTO> mappedLibrary = mapper.Map<IEnumerable<UserLibrariesDTO>>(currentLibrary);
                return Ok(mappedLibrary);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
           
        }

        ///<param name="username">
        ///an string object that holds a user name
        ///</param>
        ///<param name="songName">
        /// a string object that holds the song name
        ///</param>
        /// <summary>
        ///Search Songs
        /// </summary>
        /// 
        /// <returns>User Library objects</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("songs")]
        public ActionResult<IEnumerable<UserLibraryDTO>> SearchSongs(string username,string songName)
        {

            try
            {
                UserModel user = userDetail.GetUser(username).Result;
                if (user == null)
                {
                    return NotFound("user not found");
                }
                IEnumerable<SongLibrary> currentSongs = _library.GetSongLibrary(username, songName);
                if (currentSongs == null)
                {
                    return NotFound();
                }
                IEnumerable<UserLibraryDTO> mappedLibrary = mapper.Map<IEnumerable<UserLibraryDTO>>(currentSongs);
                return Ok(mappedLibrary);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            
        }

        ///<param name="username">
        ///an string object that holds a user name
        ///</param>
        ///<param name="songName">
        /// a string object that holds the song name
        ///</param>
        /// <summary>
        /// Add library
        /// </summary>
        /// 
        /// <returns>200 response</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPost("{songName}")]
        public async Task<IActionResult> AddLibrary(string username,string songName)
        {
            try
            {
                UserModel user = await userDetail.GetUser(username);
                if (user == null)
                {
                    return NotFound("user not found");
                }
                SongModel currentSong = await _song.GetSong(username, songName);
                if (currentSong == null)
                {
                    return NotFound();
                }
                await _library.AddToLibrary(username, currentSong.SongId);
                return Ok("Success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            
        }

        ///<param name="username">
        ///an string object that holds a user name
        ///</param>
        ///<param name="songName">
        /// a string object that holds the song name
        ///</param>
        /// <summary>
        /// Remove Song From library
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpDelete("{songName}")]
        public async Task<IActionResult> RemoveFromLibrary(string username, string songName)
        {

            try
            {
                UserModel user = await userDetail.GetUser(username);
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
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
           
        }
    }
}
