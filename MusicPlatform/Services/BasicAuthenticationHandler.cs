﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicPlatform.Model.User;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MusicPlatform.Services
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        readonly UserManager<UserModel> userManager;
        private readonly IPasswordHasher<UserModel> passwordHasher;
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, UserManager<UserModel> userManager, IPasswordHasher<UserModel> passwordHasher
            )
    : base(options, logger, encoder, clock)
        {
            this.userManager = userManager;
            this.passwordHasher = passwordHasher;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Response.Headers.Add("Authorization", "Bearer");

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header missing."));
            }

            // Get authorization key
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var authHeaderRegex = new Regex(@"Bearer (.*)");
            

            if (!authHeaderRegex.IsMatch(authorizationHeader))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization code not formatted properly."));
            }

            var authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderRegex.Replace(authorizationHeader, "$1")));
            var authSplit = authBase64.Split(Convert.ToChar(":"), 2);
            var authUsername = authSplit[0];
            var authPassword = authSplit.Length > 1 ? authSplit[1] : throw new Exception("Unable to get password");

            var currentUser = userManager.FindByNameAsync(authUsername).Result;
            var isVerify = passwordHasher.VerifyHashedPassword(currentUser, currentUser.PasswordHash, authPassword);

            if (authUsername != currentUser.UserName || isVerify.ToString() != "Success")
            {
                return Task.FromResult(AuthenticateResult.Fail("The username or password is not correct."));
            }

            var authenticatedUser = new AuthenticatedUser("BasicAuthentication", true, currentUser.UserName);
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(authenticatedUser));
            
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
        }
    }
}
