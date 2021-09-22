using Microsoft.AspNetCore.Authorization;


namespace MusicPlatform.Services
{
    public class BasicAuthorizationAttribute : AuthorizeAttribute
    {
        public BasicAuthorizationAttribute()
        {
            Policy = "BasicAuthentication";
        }
    }
}
