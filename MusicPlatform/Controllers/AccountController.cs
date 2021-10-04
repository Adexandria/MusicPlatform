﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.User;
using MusicPlatform.Model.User.Login;
using MusicPlatform.Model.User.SignUpDTO;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly UserManager<UserModel> userManager;
        private readonly IPasswordHasher<UserModel> passwordHasher;
        private readonly SignInManager<UserModel> signInManager;
        private readonly IMapper mapper;
        
        public AccountController(SignInManager<UserModel> signInManager, UserManager<UserModel> userManager, IMapper mapper, IPasswordHasher<UserModel> passwordHasher)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.passwordHasher = passwordHasher;
            this.signInManager = signInManager;
        }

        ///<param name="newUser">
        ///an object to sign up a user
        ///</param>
        /// <summary>
        /// Sign up a new user
        /// </summary>
        /// 
        /// <returns>A string status</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ActionResult> SignUp(SignUpUser newUser)
        {
            try
            {
                    var signupUser = mapper.Map<UserModel>(newUser);
                    if (newUser.Password.Equals(newUser.RetypePassword))
                    { 
                       
                        IdentityResult identity = await userManager.CreateAsync(signupUser, signupUser.PasswordHash);

                        if (identity.Succeeded)
                        {
                            await userManager.AddToRoleAsync(signupUser, "User");
                            await userManager.AddClaimAsync(signupUser, new Claim($"{signupUser.UserName }","User"));
                            var token = await EmailConfirmationToken(signupUser);
                            return this.StatusCode(StatusCodes.Status201Created, $"Welcome,{signupUser.UserName} use this {token} to verify email");
                        }
                        else
                        {
                            return BadRequest(identity.Errors);
                        }
                    }
                    return this.StatusCode(StatusCodes.Status400BadRequest, "Password not equal,retype password");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        ///<param name="newUser">
        ///an object to sign up a user
        ///</param>
        /// <summary>
        /// Sign up a new user
        /// </summary>
        /// 
        /// <returns>A string status</returns>
        //The Function creates a new user
        //using user manager library
        //then logins in the newly created user
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("signup/artist")]
        public async Task<ActionResult> SignUpArtist(SignUpArtist newUser)
        {
            try
            {
                var signupUser = mapper.Map<UserModel>(newUser);
                if (newUser.Password.Equals(newUser.RetypePassword))
                {
                     signupUser.Verified = true; 
                    IdentityResult identity = await userManager.CreateAsync(signupUser, signupUser.PasswordHash);

                    if (identity.Succeeded)
                    {
                        await userManager.AddToRoleAsync(signupUser, "Artist");
                        await userManager.AddClaimAsync(signupUser, new Claim( $"{signupUser.UserName }","Artist"));
                        var token = await EmailConfirmationToken(signupUser);
                        return this.StatusCode(StatusCodes.Status201Created, $"Welcome,{signupUser.UserName} use this {token} to verify email");
                    }
                    else
                    {
                        return BadRequest(identity.Errors);
                    }
                }
                return this.StatusCode(StatusCodes.Status400BadRequest, "Password not equal,retype password");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        ///<param name="username">
        ///\a user's username
        ///</param>
        ///<param name="token">
        ///a token to confirm user's email
        ///</param>
        /// <summary>
        ///email confrimation
        /// </summary>
        /// 
        /// <returns>A string status</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPost("{username}/emailconfirmation")]
        public async Task<ActionResult> VerifyEmailToken(Token token, string username)
        {
            try
            {
                var currentUser = await GetUser(username);
                if (currentUser == null) return NotFound("username doesn't exist");
                var result = await userManager.ConfirmEmailAsync(currentUser, token.GeneratedToken);
                if (result.Succeeded)
                {
                    return this.StatusCode(StatusCodes.Status200OK, $"Welcome,{currentUser.UserName} Email has been verified");
                }
                else
                {
                    return this.StatusCode(StatusCodes.Status400BadRequest, "Invalid Token");
                }
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        ///<param name="model">
        ///an object to login
        ///</param>
        /// <summary>
        ///Login User
        /// </summary>
        /// 
        /// <returns>A string status</returns>

        //To generate the token to reset password
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("login")]
        public async Task<ActionResult> Login(Login model)
        {
            try
            {
                
                var logindetails = mapper.Map<UserModel>(model);
                var currentUser = await userManager.FindByNameAsync(logindetails.UserName);
                if (currentUser == null) return NotFound("Username doesn't exist");
                //This verfies the user password by using IPasswordHasher interface
                var passwordVerifyResult = passwordHasher.VerifyHashedPassword(currentUser, currentUser.PasswordHash, model.Password);
                if (passwordVerifyResult.ToString() == "Success")
                {
                    await signInManager.PasswordSignInAsync(currentUser.UserName, model.Password, false, false);
                    var bearertoken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{currentUser.UserName}:{model.Password}"));
                    return Ok($"bearer:{bearertoken}");
                }

                return BadRequest("password is not correct");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }

        [NonAction]
        private async Task<string> EmailConfirmationToken(UserModel newUser)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(newUser);
            
        }

        [NonAction]
        private async Task<UserModel> GetUser(string userName)
        {
            return await userManager.FindByNameAsync(userName);
        }

      
    }
}