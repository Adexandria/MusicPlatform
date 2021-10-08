using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlatform.Controllers
{
    [Route("api/{username}/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IUserProfile _profile;
        private readonly ISong _song;
        private readonly IMapper mapper;
        private readonly IUser userDetail;
        public SearchController(IUserProfile _profile, IMapper mapper, IUser userDetail, ISong _song)
        {
            this._profile = _profile;
            this.mapper = mapper;
            this.userDetail = userDetail;
            this._song = _song;
        }

        [Authorize("BasicAuthentication")]
        [HttpPost("Follow/{follower}")]
        public async Task<ActionResult> FollowArtist([FromRoute] string username, string follower)
        {
            var currentUser = await userDetail.GetUser(username);
            if (currentUser == null)
            {
                return NotFound("User not found");
            }
            var currentFollower = await userDetail.GetUser(follower);
            if (currentFollower == null)
            {
                return NotFound("artist not found");
            }
            if (await _profile.IsFollowing(currentUser.Id, currentFollower.Id))
            {
                return BadRequest("This User is being followed by you");
            }
            await _profile.Follow(username, follower);
            return Ok($"Following {follower}");
        }

        [Authorize("BasicAuthentication")]
        [HttpDelete("Follow/{follower}")]
        public async Task<ActionResult> UnFollowArtist([FromRoute] string username, string follower)
        {
            var currentUser = await userDetail.GetUser(username);
            if (currentUser == null)
            {
                return NotFound("User not found");
            }
            var currentFollower = await userDetail.GetUser(follower);
            if (currentFollower == null)
            {
                return NotFound("artist not found");
            }
            if (await _profile.IsFollowing(currentUser.Id, currentFollower.Id))
            {
                await _profile.UnFollow(currentUser.Id, currentFollower.Id);
                return Ok($"Unfollowing {follower}");
            }
            return BadRequest("Unsuccessful, the follower isn't being followed ");
        }
    }
}
