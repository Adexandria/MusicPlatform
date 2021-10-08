using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.Library;
using MusicPlatform.Model.Library.DTO;
using MusicPlatform.Model.User.Profile.ProfileDTO;
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
        [HttpGet("users/{name}")]
        public ActionResult<IEnumerable<UsersDTO>>SearchUser([FromRoute] string username,string name)
        {
            try
            {
                var currentUser = userDetail.GetUser(username).Result;
                if (currentUser == null)
                {
                    return NotFound("User not found");
                }
                var artists = userDetail.SearchUser(name);
                var mappedArtists = mapper.Map<IEnumerable<UsersDTO>>(artists);
                return Ok(mappedArtists);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }
        /*[Authorize("BasicAuthentication")]
       [HttpGet("users")]
       public ActionResult<IEnumerable<UsersDTO>> GetUsers([FromRoute] string username)
       {
           try
           {
               var currentUser = userDetail.GetUser(username).Result;
               if (currentUser == null)
               {
                   return NotFound("User not found");
               }
               var artists = userDetail.GetUsers;
               var mappedArtists = mapper.Map<IEnumerable<UsersDTO>>(artists);
               return Ok(mappedArtists);
           }
           catch (Exception e)
           {
               return BadRequest(e.Message);

           }
       
        /*        [Authorize("BasicAuthentication")]
                [HttpGet("artists")]
                public ActionResult<IEnumerable<ArtistsDTO>> GetArtists([FromRoute] string username)
                {
                    try
                    {
                        var currentUser =  userDetail.GetUser(username).Result;
                        if (currentUser == null)
                        {
                            return NotFound("User not found");
                        }
                        var artists = userDetail.GetArtists;
                        var mappedArtists = mapper.Map<IEnumerable<ArtistsDTO>>(artists);
                        return Ok(mappedArtists);
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);

                    }
                } */
        [Authorize("BasicAuthentication")]
        [HttpGet("artists/{artistname}")]
        public ActionResult<IEnumerable<ArtistsDTO>> SearchArtist([FromRoute] string username,string artistname)
        {
            try
            {
                var currentUser = userDetail.GetUser(username).Result;
                if (currentUser == null)
                {
                    return NotFound("User not found");
                }
                var artists = userDetail.SearchArtist(artistname);
                var mappedArtists = mapper.Map<IEnumerable<ArtistsDTO>>(artists);
                return Ok(mappedArtists);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [AllowAnonymous]
        [HttpGet("songs")]
        public ActionResult<IEnumerable<SongsDTO>> SearchSong([FromRoute] string username,string songName)
        {
            try
            {
                var currentUser = userDetail.GetUser(username).Result;
                if (currentUser == null)
                {
                    return NotFound("User not found");
                }
                var songs = _song.GetSong(songName);
                var mappedSongs = mapper.Map<IEnumerable<SongsDTO>>(songs);
                return Ok(mappedSongs);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
           
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
