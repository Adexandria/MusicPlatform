using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.User;
using System;
using System.Threading.Tasks;

namespace MusicPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        readonly UserManager<UserModel> userManager;
        private readonly IPasswordHasher<UserModel> passwordHasher;
        public SettingsController(UserManager<UserModel> userManager, IPasswordHasher<UserModel> passwordHasher)
        {
            this.userManager = userManager;
            this.passwordHasher = passwordHasher;
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
        [NonAction]
        private async Task<UserModel> GetUser(string userName)
        {
            return await userManager.FindByNameAsync(userName);
        }
    }
}
