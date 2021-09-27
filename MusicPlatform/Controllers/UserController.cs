using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.User;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly UserManager<UserModel> userManager;
        private readonly IPasswordHasher<UserModel> passwordHasher;
        private readonly SignInManager<UserModel> signInManager;
        private readonly IMapper mapper;
        
        public UserController(SignInManager<UserModel> signInManager, UserManager<UserModel> userManager, IMapper mapper, IPasswordHasher<UserModel> passwordHasher)
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
        //The Function creates a new user
        //using user manager library
        //then logins in the newly created user
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
                    await signInManager.SignInAsync(currentUser, new AuthenticationProperties() { IsPersistent = false},"BasicAuthentication");
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

        ///<param name="username">
        ///\a user's username
        ///</param>
        /// <summary>
        ///reset password
        /// </summary>
        /// 
        /// <returns>A string status</returns>

        //To generate the token to reset password
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
        [Authorize("BasicAuthentication")]
        [HttpGet("{username}/password/reset")]
        public async Task<ActionResult<Token>> ResetPassword(string username)
        {
            try
            {
                var currentUser = await GetUser(username);
                if (currentUser == null) return NotFound("username doesn't exist");
                var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(currentUser);
                Token token = new Token() { GeneratedToken = passwordResetToken };
                return Ok(token);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        ///<param name="username">
        ///\a user's username
        ///</param>
        ///<param name="resetPassword">
        ///an object to reset a password
        ///</param>
        /// <summary>
        ///verify token/reset password confirmation
        /// </summary>
        /// 
        /// <returns>A string status</returns>
        //To verify the Password reset token
        //which gives access to change the user's password
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
        [HttpPost("{username}/password/verifytoken")]
        public async Task<ActionResult> VerifyPasswordToken(ResetPassword resetPassword, string username)
        {
            try
            {
                var currentUser = await GetUser(username);
                if (currentUser == null) return NotFound("username doesn't exist");
                var isVerify = passwordHasher.VerifyHashedPassword(currentUser, currentUser.PasswordHash, resetPassword.NewPassword);
                if (isVerify.ToString() == "Success") return BadRequest("Old password can't be new password");
                var isVerifyResult = await userManager.ResetPasswordAsync(currentUser, resetPassword.Token, resetPassword.NewPassword);
                if (isVerifyResult.Succeeded)
                {
                    return Ok("Password changed");
                }
                else
                {
                    return BadRequest("Try Again");
                }
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }

        ///<param name="username">
        ///\a user's username
        ///</param>
        ///<param name="name">
        ///an object used to change user's username
        ///</param>
        /// <summary>
        /// change username
        /// </summary>
        /// 
        /// <returns>A string status</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
        [Authorize("BasicAuthentication")]
        [HttpPost("{username}/change")]
        public async Task<ActionResult<UserModel>> ChangeUsername(UserName name, string username)
        {
            try
            {
                var currentUser = await GetUser(username);
                if (currentUser == null) return NotFound("username doesn't exist");
                var result = await userManager.SetUserNameAsync(currentUser, name.NewUsername);
                if (result.Succeeded)
                {
                    return this.StatusCode(StatusCodes.Status200OK, "Successfully Changed");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }

      /*  ///<param name="username">
        ///\a user's username
        ///</param>
        /// <summary>
        /// sign out user
        /// </summary>
        /// 
        /// <returns>A string status</returns>
        //To sign out a user
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
        [Authorize("BasicAuthentication")]
        [HttpPost("{username}/signout")]
        public async Task<ActionResult> Signout(string username)
        {
            try
            {
                var currentUser = await GetUser(username);
                if (currentUser == null) return NotFound("username doesn't exist");
                var principalClaim = this.User;
                if (principalClaim.Identity.IsAuthenticated)
                {
                    await signInManager.SignOutAsync();
                    return Ok("Successful");
                    principalClaim.Identity.
                }
                return BadRequest("User is signed out");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }*/

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
    
