using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.User;
using MusicPlatform.Model.User.Profile;
using MusicPlatform.Model.User.Profile.ProfileDTO;
using MusicPlatform.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlatform.Controllers
{
    [Route("api/users")]
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
   // [ApiVersion("2.0")]
    public class SearchController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUser userDetail;
        private IUserProfile _profile;

        public SearchController( IMapper mapper, IUser userDetail, IUserProfile _profile)
        {
            this.mapper = mapper;
            this.userDetail = userDetail;
            this._profile =_profile;
        }

        ///<param name="name">
        /// a string object holds a user name
        ///</param>
        /// <summary>
        /// Search user
        /// </summary>
        /// <returns>
        /// return user object
        /// </returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("search")]
        public ActionResult<IEnumerable<UsersDTO>>SearchUser([FromQuery]string name)
        {
            try
            {
                IEnumerable<UserModel> users = userDetail.SearchUser(name);
                IEnumerable<UsersDTO> mappedUsers = mapper.Map<IEnumerable<UsersDTO>>(users);
                return Ok(mappedUsers);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        ///<param name="name">
        /// a string object holds a user name
        ///</param>
        /// <summary>
        /// Search user
        /// </summary>
        /// <returns>
        /// return artist object
        /// </returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("search/artists")]
        public ActionResult<IEnumerable<ArtistsDTO>> SearchArtist([FromQuery]string name)
        {
            try
            {
                IEnumerable<UserModel> artists = userDetail.SearchArtist(name);
                IEnumerable<ArtistsDTO> mappedArtists = mapper.Map<IEnumerable<ArtistsDTO>>(artists);
                return Ok(mappedArtists);
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
        /// Get User Profile
        /// </summary>
        /// 
        /// <returns>UserProfile Object</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile([FromQuery]string username)
        {
            try
            {
                UserModel currentuser = await userDetail.GetUser(username);
                if (currentuser == null)
                {
                    return NotFound("User not found");
                }
                UserProfile currentprofile = await _profile.GetUserProfile(username);
                if (currentprofile.User.Verified)
                {
                    var mappedArtistProfile = mapper.Map<ArtistProfileDTO>(currentprofile);
                    return Ok(mappedArtistProfile);
                }
                UserProfileDTO mappedUserProfile = mapper.Map<UserProfileDTO>(currentprofile);
                return Ok(mappedUserProfile);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }



    }
}
