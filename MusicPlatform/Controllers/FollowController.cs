using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.User;
using MusicPlatform.Services;
using System;
using System.Threading.Tasks;

namespace MusicPlatform.Controllers
{
    [Route("api/{username}/[controller]")]
    [ApiController]
    [Authorize("BasicAuthentication")]
    [ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    public class FollowController : ControllerBase
    {
        private readonly IUserProfile _profile;
        private readonly IUser userDetail;
        public FollowController(IUser userDetail, IUserProfile _profile)
        { 
            this.userDetail = userDetail;
            this._profile = _profile;
        }

        ///<param name="username">
        ///an string object 
        ///</param>
        ///<param name="follower">
        /// a string object
        ///</param>
        /// <summary>
        /// Follow user
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPost("{follower}")]
        public async Task<ActionResult> Follow([FromRoute] string username, string follower)
        {

            try
            {
                UserModel currentUser = await userDetail.GetUser(username);
                if (currentUser == null)
                {
                    return NotFound("User not found");
                }
                UserModel currentFollower = await userDetail.GetUser(follower);
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

        ///<param name="username">
        ///an strig object 
        ///</param>
        ///<param name="follower">
        /// a string object
        ///</param>
        /// <summary>
        /// Unfollow user
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpDelete("{follower}")]
        public async Task<ActionResult> UnFollow([FromRoute] string username, string follower)
        {

            try
            {
                UserModel currentUser = await userDetail.GetUser(username);
                if (currentUser == null)
                {
                    return NotFound("User not found");
                }
                UserModel currentFollower = await userDetail.GetUser(follower);
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
