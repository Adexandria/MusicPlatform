using AutoMapper;
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
    [Route("api/{username}/{songName}/[controller]")]
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
            var currentUser = await userDetail.GetUser(username);
            if (currentUser == null)
            {
                return NotFound("User not found");
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

        [HttpPut]
        public async Task<IActionResult> UpdateSongCredit([FromRoute] string username, [FromRoute] string songName, CreditUpdate updateCredit)
        {
            var currentUser = await userDetail.GetUser(username);
            if (currentUser == null)
            {
                return NotFound("User not found");
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

        [HttpDelete]
        public async Task<IActionResult> DeleteSongCredit([FromRoute] string username, [FromRoute] string songName)
        {
            var currentUser = await userDetail.GetUser(username);
            if (currentUser == null)
            {
                return NotFound("User not found");
            }
            var currentSong = await _song.GetSong(username, songName);
            if (currentSong == null)
            {
                return NotFound("Song doesn't exist");
            }
            await _song.DeleteCredit(currentSong.SongId);
            return Ok("Success");
        }
    }
}
