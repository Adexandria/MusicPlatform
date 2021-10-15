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
    [Authorize("BasicAuthentication")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUser userDetail;
        public SearchController( IMapper mapper, IUser userDetail)
        {
            this.mapper = mapper;
            this.userDetail = userDetail;
        }
       
        
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

        

      
     
    }
}
