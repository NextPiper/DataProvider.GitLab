using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using DataProvider.GitLab.GitLabData;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DataProvider.GitLab.Controllers
{
    [ApiController]
    [Route("GitLab")]
    public class GitLabController : Controller
    {
        private const string GITLAB_BASE_URL = "https://gitlab.com/api/v4/";
        private const string GITLAB_PROJECT_URL = GITLAB_BASE_URL + "projects/";
        
        private const string url = "https://gitlab.com/api/v4/users";
        
        [HttpGet]
        [Route("projects")]
        public async Task<IActionResult> GetProjects()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var result = await httpClient.GetAsync(GetUserProjectsUrl(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value));

            if (!result.IsSuccessStatusCode)
            {
                return new ObjectResult(result);
            }

            var content = await result.Content.ReadAsStringAsync();
            var projects = JsonConvert.DeserializeObject<IEnumerable<GitLabProject>>(content);
            
            return new ObjectResult(projects);
        }

        [HttpGet]
        [Route("projects/{id}")]
        public async Task<IActionResult> GetProject(string id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var url = $"{GITLAB_PROJECT_URL}{id}";
            
            var result = await httpClient.GetAsync(url);

            if (!result.IsSuccessStatusCode)
            {
                return new ObjectResult(result);
            }

            var content = await result.Content.ReadAsStringAsync();
            var projects = JsonConvert.DeserializeObject<GitLabProject>(content);
            
            return new ObjectResult(projects);
        }


        [HttpGet]
        [Route("projects/{id}/hooks")]
        public async Task<IActionResult> GetProjectHooks(string id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var url = $"{GITLAB_PROJECT_URL}{id}/hooks";
            
            var result = await httpClient.GetAsync(url);

            if (!result.IsSuccessStatusCode)
            {
                return new ObjectResult(result);
            }

            var content = await result.Content.ReadAsStringAsync();
            var projects = JsonConvert.DeserializeObject<IEnumerable<GitLabHook>>(content);
            
            return new ObjectResult(projects);
        }

        [HttpGet]
        [Route("projects/{id}/hooks/{hookId}")]
        public async Task<IActionResult> GetProjectHook(string id, string hookId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var result = await httpClient.GetAsync($"{GITLAB_PROJECT_URL}{id}/hooks/{hookId}");

            if (!result.IsSuccessStatusCode)
            {
                return new ObjectResult(result);
            }

            var content = await result.Content.ReadAsStringAsync();
            var projects = JsonConvert.DeserializeObject<GitLabHook>(content);
            
            return new ObjectResult(projects);
        }

        [HttpPost]
        [Route("projects/{id}/hooks")]
        public async Task<IActionResult> Posthook(string id, string hookUrl)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            
            var webhook = Url.Action(nameof(WebhookController.GitlabHook), "Webhook", null);

            if (hookUrl != null)
            {
                webhook = hookUrl;
            }
            
            var result = await httpClient.PostAsync($"{GITLAB_PROJECT_URL}{id}/hooks?id={id}&url={webhook}&push_events=true", null);

            if (!result.IsSuccessStatusCode)
            {
                return new ObjectResult(result);
            }

            return StatusCode(201);
        }

        [HttpDelete]
        [Route("projcts/{id}/hooks/{hookId}")]
        public async Task<IActionResult> DeleteHook(string id, string hookId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var url = $"{GITLAB_PROJECT_URL}{id}/hooks/{hookId}";
            
            var result = await httpClient.DeleteAsync(url);

            if (!result.IsSuccessStatusCode)
            {
                return new ObjectResult(result);
            }

            return StatusCode(201);
        }

        [HttpGet]
        [Route("commit")]
        public async Task<IActionResult> GetCommit()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            
            var url = "https://gitlab.com/ulsan16/hookproject/-/commit/b48168d736a9671f3ed1a223e011bcd49b8d522e";

            var result = await httpClient.GetAsync(url);

            if (!result.IsSuccessStatusCode)
            {
                return new ObjectResult(result);
            }

            return StatusCode(200);
        }


        private string GetProjectHooksUrl()
        {
            return $"https://gitlab.com/api/v4/hooks";  
        }
        
        private string GetUserProjectsUrl(string userId)
        {
            return $"https://gitlab.com/api/v4/users/{userId}/projects"; 
        }
    }
}