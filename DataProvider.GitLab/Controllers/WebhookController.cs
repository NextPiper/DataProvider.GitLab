using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using DataProvider.GitLab.GitLabData;
using DataProvider.GitLab.Managers.GitLabHandler;
using DataProvider.GitLab.Managers.RabbitMQManager;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DataProvider.GitLab.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class WebhookController : Controller
    {
        private readonly IGitLabHandler _gitLabHandler;
        private const string GITLAB_BASE_URL = "https://gitlab.com/api/v4/";
        private const string GITLAB_PROJECT_URL = GITLAB_BASE_URL + "projects/";
        
        private readonly IEnumerable<string> validPrefix = new List<string>{"cs", "js", "py"};

        public WebhookController(IGitLabHandler gitLabHandler)
        {
            _gitLabHandler = gitLabHandler;
        }
        
        [HttpPost]
        [Route("gitlab-hook")]
        public async Task<IActionResult> GitlabHook([FromBody] GitLabPushEventRequestModel requestModel)
        {
            Console.WriteLine($"GitLab webhook received at - {DateTime.Now.ToString()}");
            Console.WriteLine($"Authorized: {User.Identity.IsAuthenticated} - As: {User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value}");
           
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            // Start own thread to handle the received push event
            _gitLabHandler.Handle_PushEvent(requestModel, accessToken);
            
            // return 200 to indicate successful delivery of the webhook
            return StatusCode(200);
        }
    }
}