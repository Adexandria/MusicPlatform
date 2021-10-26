﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.Library;
using MusicPlatform.Model.Library.DTO;
using MusicPlatform.Model.User;
using MusicPlatform.Services;
using System;
using System.Threading.Tasks;
using Text_Speech.Services;

namespace MusicPlatform.Controllers
{
    [Route("api/{username}/[controller]")]
    [Authorize("BasicAuthentication",Roles ="Artist")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly IBlob _blob;
        private readonly ISong _song;
        private readonly UserManager<UserModel> _userManager;
        private readonly IMapper mapper;
        private readonly IUser userDetail;
        public SongController(IBlob _blob, IMapper mapper, IUser userDetail, ISong _song, UserManager<UserModel> _userManager)
        {
            this._blob = _blob;
            this.mapper = mapper;
            this.userDetail = userDetail;
            this._song = _song;
            this._userManager = _userManager;
        }

        [HttpPost]
        public async Task<IActionResult> UploadSong(string username,[FromForm]SongCreate newSong)
        {
            try
            {
                var currentUser = await userDetail.GetUser(username);
                if (currentUser == null)
                {
                    return NotFound("Username not found");
                }
                await _blob.Upload(newSong.Song);
                string url = _blob.GetUri(newSong.Song.FileName).ToString();
                var mappedSong = mapper.Map<SongModel>(newSong);
                mappedSong.SongUrl = url;
                await _song.AddSong(mappedSong, username);
                return Ok("Success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
                
            }
            
        }

        [HttpPut("{songName}")]
        public async Task<IActionResult> UpdateSong(string username,string songName, [FromForm] SongUpdate updatedSong)
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
                    return NotFound();
                }
                await _blob.Upload(updatedSong.Song);
                string url = _blob.GetUri(updatedSong.Song.FileName).ToString();
                var mappedSong = mapper.Map<SongModel>(updatedSong);
                mappedSong.SongUrl = url;
                await _song.UpdateSong(mappedSong, songName, username);
                return Ok("Success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
           
        }

        [HttpDelete("{songName}")]
        public async Task<IActionResult> DeleteSong(string username, string songName)
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
                    return NotFound();
                }
                await _song.DeleteSong(username, songName);
                return Ok("Success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            
        }


        [HttpPost("{songName}/image")]
        public async Task<IActionResult> AddSongImage([FromRoute]string username,[FromRoute]string songName,IFormFile image)
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
                await _blob.Upload(image);
                string url = _blob.GetUri(image.FileName).ToString();
                await _song.AddImage(url, currentSong.SongId);
                return Ok("Success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            
        }
        [HttpPut("{songName}/image")]
        public async Task<IActionResult> UpdateSongImage([FromRoute] string username, [FromRoute] string songName, IFormFile image)
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
                await _blob.Upload(image);
                string url = _blob.GetUri(image.FileName).ToString();
                await _song.UpdateImage(url, currentSong.SongId);
                return Ok("Success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
           
        }

        [HttpDelete("{songName}/image")]
        public async Task<IActionResult> DeleteSongImage([FromRoute] string username, [FromRoute] string songName)
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
                await _song.DeleteImage(currentSong.SongId);
                return Ok("Success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            
        }


    }
}
