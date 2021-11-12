using System;
using AutoMapper;
using System.Text;
using System.Threading.Tasks;
using MusicPlatform.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MusicPlatform.Model.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MusicPlatform.Model.User.Login;
using MusicPlatform.Model.User.SignUpDTO;
using Microsoft.AspNetCore.Authorization;



namespace MusicPlatform.Controllers
{
   
    [ApiController]
    [ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        readonly UserManager<UserModel> userManager;
        readonly IPasswordHasher<UserModel> passwordHasher;
        readonly SignInManager<UserModel> signInManager;
        readonly IMapper mapper;
        readonly IUserProfile _profile;
        readonly IUser user;
        

        public AccountController(SignInManager<UserModel> signInManager, UserManager<UserModel> userManager, 
            IMapper mapper, IPasswordHasher<UserModel> passwordHasher, IUserProfile _profile, IUser user)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.passwordHasher = passwordHasher;
            this.signInManager = signInManager;
            this._profile = _profile;
            this.user = user;
        }


        ///<param name="newUser">
        ///an object to sign up a user
        ///</param>
        /// <summary>
        /// Sign up a new user
        /// </summary>
        /// 
        /// <returns>200 response</returns>
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
                    UserModel signupUser = mapper.Map<UserModel>(newUser);
                    if (newUser.Password.Equals(newUser.RetypePassword))
                    {                    
                        IdentityResult identity = await userManager.CreateAsync(signupUser, signupUser.PasswordHash);
                        if (identity.Succeeded)
                        {
                            await userManager.AddToRoleAsync(signupUser, "User");
                            await userManager.AddClaimAsync(signupUser, new Claim($"{signupUser.UserName }","User"));
                            await user.AddUser(signupUser);
                            await _profile.AddUserProfile(signupUser.UserName);
                            string token = await EmailConfirmationToken(signupUser);
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


        ///<param name="newArtist">
        ///an object to sign up a user
        ///</param>
        /// <summary>
        /// Sign up a new user
        /// </summary>
        /// 
        /// <returns>200 response</returns>
        //The Function creates a new user
        //using user manager library
        //then logins in the newly created user
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("signup/artist")]
        public async Task<ActionResult> SignUpArtist(SignUpArtist newArtist)
        {
            try
            {
                UserModel signupUser = mapper.Map<UserModel>(newArtist);
                if (newArtist.Password.Equals(newArtist.RetypePassword))
                {
                    signupUser.Verified = true; 
                    IdentityResult identity = await userManager.CreateAsync(signupUser, signupUser.PasswordHash);

                    if (identity.Succeeded)
                    {
                        await userManager.AddToRoleAsync(signupUser, "Artist");
                        await userManager.AddClaimAsync(signupUser, new Claim( $"{signupUser.UserName }","Artist"));
                        await user.AddUser(signupUser);
                        await _profile.AddUserProfile(signupUser.UserName);
                        string token = await EmailConfirmationToken(signupUser);
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
        /// <returns>200 response</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("{username}/emailconfirmation")]
        public async Task<ActionResult> VerifyEmailToken(Token token, string username)
        {
            try
            {
                UserModel currentUser = await GetUser(username);
                if (currentUser == null) return NotFound("username doesn't exist");
                IdentityResult result = await userManager.ConfirmEmailAsync(currentUser, token.GeneratedToken);
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

        ///<param name="user">
        ///an object to login
        ///</param>
        /// <summary>
        ///Login User
        /// </summary>
        /// 
        /// <returns>200 response</returns>

        //To generate the token to reset password
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("login")]
        public async Task<ActionResult> Login(Login user)
        {
            try
            {
                
                UserModel logindetails = mapper.Map<UserModel>(user);
                UserModel currentUser = await userManager.FindByNameAsync(logindetails.UserName);
                if (currentUser == null) 
                { 
                    return NotFound("username or password is not correct");
                }

                //This verfies the user password by using IPasswordHasher interface
                PasswordVerificationResult passwordVerifyResult = passwordHasher.VerifyHashedPassword(currentUser, currentUser.PasswordHash, user.Password);
                if (passwordVerifyResult.ToString() == "Success")
                {
                    await signInManager.PasswordSignInAsync(currentUser.UserName, currentUser.PasswordHash, false, false);
                    string bearertoken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{currentUser.UserName}:{user.Password}"));
                    return Ok($"bearer:{bearertoken}");
                }

                return BadRequest("username or password is not correct");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        ///<param name="username">
        ///a user name string object
        ///</param>
        /// <summary>
        ///sign out User
        /// </summary>
        /// 
        /// <returns>200 response</returns>

        //To generate the token to reset password
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize("BasicAuthentication")]
        [Produces("application/json")]
        [HttpPost("{username}/signout")]
        public async Task<ActionResult> SignOut(string username)
        {
            try
            {
                UserModel currentUser = await userManager.FindByNameAsync(username);
                if (currentUser == null)
                {
                    return NotFound("Username doesn't exist");
                }
                if (signInManager.IsSignedIn(this.User))
                {
                    await signInManager.SignOutAsync();
                    return Ok("Signed out");
                }
                return BadRequest("This user isn't signed in");
            }
            catch ( Exception e)
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