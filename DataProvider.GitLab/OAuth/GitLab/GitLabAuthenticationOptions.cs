using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace DataProvider.GitLab.OAuth.GitLab
{
    public class GitLabAuthenticationOptions : OAuthOptions
    {
        /// <summary>
        /// Initializes a new <see cref="GitLabAuthenticationOptions"/>.
        /// </summary>
        public GitLabAuthenticationOptions()
        {
            CallbackPath = new PathString(GitLabAuthenticationDefaults.CallbackPath);
            AuthorizationEndpoint = GitLabAuthenticationDefaults.AuthorizationEndpoint;
            TokenEndpoint = GitLabAuthenticationDefaults.TokenEndpoint;
            UserInformationEndpoint = GitLabAuthenticationDefaults.UserInformationEndpoint;
            ClaimsIssuer = GitLabAuthenticationDefaults.ClaimsIssuer;
            Scope.Add("openid");
            Scope.Add("profile");
            Scope.Add("email");
            Scope.Add("read_user");
            Scope.Add("api");
            Scope.Add("sudo");
            Scope.Add("read_repository");
            Scope.Add("write_repository");
            Scope.Add("read_registry");

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
            ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            ClaimActions.MapJsonKey(GitLabAuthenticationConstants.Claims.Name, "name");
            ClaimActions.MapJsonKey(GitLabAuthenticationConstants.Claims.Avatar, "avatar_url");
            ClaimActions.MapJsonKey(GitLabAuthenticationConstants.Claims.Url, "web_url");
        }

        /// <summary>
        /// access_type. Set to 'offline' to request a refresh token.
        /// </summary>
        public string AccessType { get; set; }
    }
}