using AutoMapper;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.Library;
using MusicPlatform.Model.Library.DTO;
using MusicPlatform.Model.User.Profile.DTO;
using MusicPlatform.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Text_Speech.Services;

namespace MusicPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    //[ApiVersion("2.0")]
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

        ///<param name="songName">
        /// an object to create a new song model
        ///</param>
        /// <summary>
        /// Get Available songs
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet]
        public ActionResult<IEnumerable<SongsDTO>> GetSongs(string songName)
        {
            try
            {
                IEnumerable<SongModel> songs = _song.GetSong(songName);
                IEnumerable<SongsDTO> mappedSongs = mapper.Map<IEnumerable<SongsDTO>>(songs);
                return Ok(mappedSongs);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }

        ///<param name="artist">
        ///an string object that holds a user name
        ///</param>
        ///<param name="songName">
        /// an object to create a new song model
        ///</param>
        /// <summary>
        ///Get Artist Song
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{songName}")]
        public async Task<ActionResult<SongDTO>> GetArtistSong(string artist,string songName)
        {
            try
            {
                if(Request.Cookies["Download"] == null)
                {
                    Response.Cookies.Append("Download", "4");
                }

                SongModel song = await _song.GetSong(artist,songName);
                if(song == null)
                {
                    return NotFound("Song not found");
                }
                SongDTO mappedSongs = mapper.Map<SongDTO>(song);
                return Ok(mappedSongs);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        ///<param name="artist">
        ///an string object that holds a user name
        ///</param>
        ///<param name="songName">
        /// an object to create a new song model
        ///</param>
        /// <summary>
        /// Update an existing song credit
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{songName}/Download")]
        public async Task<ActionResult<SongDTO>> DownloadSong(string artist, string songName)
        {
            try
            {
                bool isSigned = IsSignedIn();
                if (!isSigned)
                {
                    return BadRequest("You reached your limit, sign in");
                }

                SongModel song = await _song.GetSong(artist, songName);
                if (song == null)
                {
                    return NotFound("Song not found");
                }
                
                await  _song.DownloadSong(artist,songName);
                Azure.Response<BlobDownloadInfo> file = await _blob.DownloadFile(song.SongUrl);
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
            string x = Request.Cookies["Download"];
            int y = Convert.ToInt32(x);
            if (y == 0)
            {
                return false;
            }
            string value = (y - 1).ToString();
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
