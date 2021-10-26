using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MusicPlatform.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public class Authenticate
    {
        readonly UserManager<UserModel> userManager;
        private readonly IPasswordHasher<UserModel> passwordHasher;
        public Authenticate(UserManager<UserModel> userManager, IPasswordHasher<UserModel> passwordHasher)
        {
            this.passwordHasher = passwordHasher;
            this.userManager = userManager;
        }
        public Task<AuthenticateResult> AuthenticateUser(string authorizationHeader)
        {
            var authBearerRegex = new Regex(@"Bearer (.*)");


            if (!authBearerRegex.IsMatch(authorizationHeader))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization code not formatted properly."));
            }

            var authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authBearerRegex.Replace(authorizationHeader, "$1")));
            var authSplit = authBase64.Split(Convert.ToChar(":"), 2);
            var authUsername = authSplit[0];
            var authPassword = authSplit.Length > 1 ? authSplit[1] : throw new Exception("Unable to get password");
            var user = VerifyUser(authUsername, authPassword).Result;
            if (user != "Success")
            {
                return Task.FromResult(AuthenticateResult.Fail("The username or password is not correct."));
            }
            var role = GetUserRole(authUsername).Result;
            var authenticatedUser = new AuthenticatedUser("BasicAuthentication", true, authUsername);
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, role)
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(authenticatedUser,claims));  
            AuthenticationProperties properties = new AuthenticationProperties
            {
                AllowRefresh = false
            };
            AuthenticationTicket ticket = new AuthenticationTicket(claimsPrincipal,"BasicAuthentication");
            return Task.FromResult(AuthenticateResult.Success(ticket));  
        }

        private async Task<string> VerifyUser(string username,string password)
        {
            var currentUser = await userManager.FindByNameAsync(username);
            if (currentUser != null)
            {
                var isVerify = passwordHasher.VerifyHashedPassword(currentUser, currentUser.PasswordHash, password);

                if (isVerify.ToString() != "Success")
                {
                    return "Success";
                }
            }
            return "Failed";
        }
        private async Task<string> GetUserRole(string username)
        {
            var currentUser = await userManager.FindByNameAsync(username);
            if (currentUser != null)
            {
                var roles = await userManager.GetRolesAsync(currentUser);
                return roles[0];
            }
            return null;
        }
        

        
    }
}

