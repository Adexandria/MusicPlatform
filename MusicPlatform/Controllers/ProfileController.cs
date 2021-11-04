using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.User.Profile.ProfileDTO;
using MusicPlatform.Services;
using System;
using System.Threading.Tasks;
using Text_Speech.Services;

namespace MusicPlatform.Controllers
{
    [Route("api/{username}/[controller]")]
    [Authorize("BasicAuthentication")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserProfile _profile;
        private readonly IBlob _blob;
        private readonly IMapper mapper;
        private readonly IUser userDetail;
        public ProfileController(IUserProfile _profile, IMapper mapper, IBlob _blob, IUser userDetail)
        {
            this._profile = _profile;
            this.mapper = mapper;
            this._blob = _blob;
            this.userDetail = userDetail;
        }
        ///<param name="username">
        ///an string object that holds a user name
        ///</param>
        /// <summary>
        /// Get User Profile
        /// </summary>
        /// 
        /// <returns>UserProfile Object</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile(string username)
        {
            try
            {
                var currentuser = await userDetail.GetUser(username);
                if (currentuser == null)
                {
                    return NotFound("User not found");
                }
                var currentprofile = await _profile.GetUserProfile(username);
                if (currentprofile.User.Verified)
                {
                    var mappedArtistProfile = mapper.Map<ArtistProfileDTO>(currentprofile);
                    return Ok(mappedArtistProfile);
                }
                var mappedUserProfile = mapper.Map<UserProfileDTO>(currentprofile);
                return Ok(mappedUserProfile);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        ///<param name="username">
        ///an string object that holds a user name
        ///</param>
        ///<param name="image">
        /// a FormFile that holds the image
        ///</param>
        /// <summary>
        /// Add Image
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPost("image")]
        public async Task<IActionResult> AddUserImage([FromRoute] string username, IFormFile image)
        {
            try
            {
                var currentuser = await userDetail.GetUser(username);
                if (currentuser == null)
                {
                    return NotFound("User not found");
                }
                if (image == null)
                {
                    return NotFound("Image not found");
                }

                await _blob.UploadImage(image);
                string url = _blob.GetUri(image.FileName).ToString();
                await _profile.AddImage(username, url);
                return Ok("Successful");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        ///<param name="username">
        ///an string object that holds a user name
        ///</param>
        ///<param name="image">
        /// a FormFile that holds the image
        ///</param>
        /// <summary>
        /// Update User Profile Image
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPut("image")]
        public async Task<ActionResult> UpdateUserImage([FromRoute] string username, IFormFile image)
        {
            try
            {
                var currentuser = await userDetail.GetUser(username);
                if (currentuser == null)
                {
                    return NotFound("User not found");
                }
                if (image == null)
                {
                    return NotFound("Image not found");
                }

                await _blob.UploadImage(image);
                string url = _blob.GetUri(image.FileName).ToString();
                await _profile.UpdateUserImage(url, username);
                return Ok("Successful");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }

        ///<param name="username">
        ///an string object that holds a user name
        ///</param>
        /// <summary>
        /// Delete User Profile Image
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpDelete("image")]
        public async Task<ActionResult> DeleteUserImage([FromRoute] string username)
        {
            try
            {
                var currentuser = await userDetail.GetUser(username);
                if (currentuser == null)
                {
                    return NotFound("User not found");
                }
                var currentImage = await _profile.GetImage(username);
                if (currentImage != null)
                {
                    await _profile.DeleteImage(username);
                    return Ok("Sucessful");
                }
                return NotFound("Image doesn't exist");

            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

       
    }
}


