using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Services;
using System;
using System.Threading.Tasks;

namespace MusicPlatform.Controllers
{
    [Route("api/{username}/[controller]")]
    [ApiController]
    [Authorize("BasicAuthentication")]
    public class FollowController : ControllerBase
    {
        private readonly IUserProfile _profile;
        private readonly IUser userDetail;
        public FollowController(IUser userDetail, IUserProfile _profile)
        { 
            this.userDetail = userDetail;
            this._profile = _profile;
        }
        [HttpPost("{follower}")]
        public async Task<ActionResult> FollowArtist([FromRoute] string username, string follower)
        {

            try
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
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            
        }


        [HttpDelete("{follower}")]
        public async Task<ActionResult> UnFollowArtist([FromRoute] string username, string follower)
        {

            try
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
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            
        }
    }
}
