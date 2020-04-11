using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using DataProvider.GitLab.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DataProvider.GitLab.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : Controller
    {

        public AccountController()
        {
        }
        
        [HttpGet]
        [Route("externallogin")]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = "/")
        {
            // Create a redirect url for the external login
            string redirectUrl = Url.Action(nameof(ExternalLoginCallback), "account", new { ReturnUrl = returnUrl});

            // Make a challenge towards the 
            return Challenge(new AuthenticationProperties {RedirectUri = redirectUrl}, provider);
        }

        [HttpGet]
        [Route("externallogincallbacks")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/account", string remoteError = null)
        {
            if (remoteError != null)
            {
                Console.WriteLine($"Error from external provider: {remoteError}");
                return StatusCode(500, $"Error from external provider: {remoteError}");
            }
            
            // Save accessToken and refreshToken for use later <---
            
            
            // Sign in the user with the external login provider
            Console.WriteLine("Signed in with gitlabbers!");
            return new ObjectResult("Signed in with gitlabbers");
        }
        
        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout(string returnUrl = "/home")
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            //await _signInManager.SignOutAsync(); // <--- If we should have a local storage create the signInManager 

            return Redirect(returnUrl);
        }

        [HttpGet]
        [Route("externallogindata")]
        public async Task<IActionResult> Data()
        {
            if (User.Identity.IsAuthenticated)
            {
                return new ObjectResult(new
                {
                    GitLabName = User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value,
                    GitLabId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                    AccessToken = await HttpContext.GetTokenAsync("access_token"),
                    RefreshToken = await HttpContext.GetTokenAsync("refresh_token")
                });
            }
            return new ObjectResult(User);
        }
    }
}