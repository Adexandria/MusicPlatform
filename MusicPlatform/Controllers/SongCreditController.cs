using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.Library;
using MusicPlatform.Model.Library.DTO;
using MusicPlatform.Model.User;
using MusicPlatform.Services;
using System;
using System.Threading.Tasks;

namespace MusicPlatform.Controllers
{
    [Route("api/{username}/{songName}/credit")]
    [Authorize("BasicAuthentication", Roles = "Artist")]
    [ApiController]
    [ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    public class SongCreditController : ControllerBase
    {
        private readonly ISong _song;
        private readonly IMapper mapper;
        private readonly IUser userDetail;
        public SongCreditController(IUser userDetail, IMapper mapper, ISong _song)
        {
            this.userDetail = userDetail;
            this.mapper = mapper;
            this._song = _song;
        }

        ///<param name="username">
        ///an string object that holds a user name
        ///</param>
        ///<param name="songName">
        /// an object holds a song name
        ///</param>
        ///<param name="newCredit">
        /// an object to create a new credit model
        ///</param>
        /// <summary>
        /// Add a song credit
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> AddSongCredit([FromRoute] string username, [FromRoute] string songName, CreditCreate newCredit)
        {
            try
            {
                UserModel currentUser = await userDetail.GetUser(username);
                if (currentUser == null)
                {
                    return NotFound("Username not found");
                }
                SongModel currentSong = await _song.GetSong(username, songName);
                if (currentSong == null)
                {
                    return NotFound("Song doesn't exist");
                }
                CreditModel mappedCredit = mapper.Map<CreditModel>(newCredit);
                await _song.AddCredit(currentSong.SongId, mappedCredit);
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
        /// an object to create a new song model
        ///</param>
        ///<param name="updateCredit">
        /// an object to update a credit model
        ///</param>
        /// <summary>
        /// Update an existing song credit
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPut]
        public async Task<IActionResult> UpdateSongCredit([FromRoute] string username, [FromRoute] string songName, CreditUpdate updateCredit)
        {

            try
            {
                UserModel currentUser = await userDetail.GetUser(username);
                if (currentUser == null)
                {
                    return NotFound("Username not found");
                }
                SongModel currentSong = await _song.GetSong(username, songName);
                if (currentSong == null)
                {
                    return NotFound("Song doesn't exist");
                }
                CreditModel mappedCredit = mapper.Map<CreditModel>(updateCredit);
                await _song.UpdateCredit(mappedCredit, currentSong.SongId);
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
        /// an object to create a new song model
        ///</param>
        /// <summary>
        /// Update an existing song credit
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpDelete]
        public async Task<IActionResult> DeleteSongCredit([FromRoute] string username, [FromRoute] string songName)
        {

            try
            {
                UserModel currentUser = await userDetail.GetUser(username);
                if (currentUser == null)
                {
                    return NotFound("Username not found");
                }
                SongModel currentSong = await _song.GetSong(username, songName);
                if (currentSong == null)
                {
                    return NotFound("Song doesn't exist");
                }
                await _song.DeleteCredit(currentSong.SongId);
                return Ok("Success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            
        }
    }
}
