using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.Library;
using MusicPlatform.Model.Library.DTO;
using MusicPlatform.Services;
using System;
using System.Threading.Tasks;

namespace MusicPlatform.Controllers
{
    [Route("api/{username}/{songName}/[controller]")]
    [Authorize("BasicAuthentication", Roles = "Artist")]
    [ApiController]
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

        [HttpPost]
        public async Task<IActionResult> AddSongCredit([FromRoute] string username, [FromRoute] string songName, CreditCreate newCredit)
        {
            try
            {
                var currentUser = await userDetail.GetUser(username);
                if (currentUser == null)
                {
                    return NotFound("Username not found");
                }
                var currentSong = await _song.GetSong(username, songName);
                if (currentSong == null)
                {
                    return NotFound("Song doesn't exist");
                }
                var mappedCredit = mapper.Map<CreditModel>(newCredit);
                await _song.AddCredit(currentSong.SongId, mappedCredit);
                return Ok("Success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSongCredit([FromRoute] string username, [FromRoute] string songName, CreditUpdate updateCredit)
        {

            try
            {
                var currentUser = await userDetail.GetUser(username);
                if (currentUser == null)
                {
                    return NotFound("Username not found");
                }
                var currentSong = await _song.GetSong(username, songName);
                if (currentSong == null)
                {
                    return NotFound("Song doesn't exist");
                }
                var mappedCredit = mapper.Map<CreditModel>(updateCredit);
                await _song.UpdateCredit(mappedCredit, currentSong.SongId);
                return Ok("Success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSongCredit([FromRoute] string username, [FromRoute] string songName)
        {

            try
            {
                var currentUser = await userDetail.GetUser(username);
                if (currentUser == null)
                {
                    return NotFound("Username not found");
                }
                var currentSong = await _song.GetSong(username, songName);
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
