﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.User.Profile.ProfileDTO;
using MusicPlatform.Services;
using System;
using System.Collections.Generic;


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

        ///<param name="username">
        ///an string object that holds a user name
        ///</param>
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


        ///<param name="username">
        ///an string object that holds a user name
        ///</param>
        ///<param name="artistname">
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
