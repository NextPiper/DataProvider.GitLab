using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using DataProvider.GitLab.GitLabData;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using DataProvider.GitLab.Managers.RabbitMQManager;
using DataProvider.GitLab.Managers.RabbitMQManager.Messages;

namespace DataProvider.GitLab.Managers.GitLabHandler
{
    public interface IGitLabHandler
    {
        Task Handle_PushEvent(GitLabPushEventRequestModel @event, string accessToken);
    }
    public class GitLabHandler : IGitLabHandler
    {
        private readonly IRabbitMQManager _rabbitMqManager;
        private const string GITLAB_BASE_URL = "https://gitlab.com/api/v4/";
        private const string GITLAB_PROJECT_URL = GITLAB_BASE_URL + "projects/";
        
        public GitLabHandler(IRabbitMQManager rabbitMqManager)
        {
            _rabbitMqManager = rabbitMqManager;
        }
        
        public async Task Handle_PushEvent(GitLabPushEventRequestModel @event, string accessToken)
        {
            // Initialize rabbitMQClient
            await _rabbitMqManager.InitClient();
            var prepareDownload = new HashSet<string>();

            foreach (var commit in @event.commits)
            {
                foreach (var addedFile in commit.added)
                {
                    if (addedFile.EndsWith(".cs") || addedFile.EndsWith(".js"))
                    {
                        prepareDownload.Add(addedFile);
                    }
                }

                foreach (var modifiedFile in commit.modified)
                {
                    if (modifiedFile.EndsWith(".cs") || modifiedFile.EndsWith(".js"))
                    {
                        prepareDownload.Add(modifiedFile);
                    }
                }
            }
            
            Console.WriteLine($"Preparing to download {prepareDownload.Count} files");
            
            // Modified and added files
            var list = new List<GitLabFile>();
            
            var httpClient = new HttpClient();
            /*httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);*/

            Console.WriteLine($"httpClient prepared - Starting download of files {list.Count}/{prepareDownload.Count}");
            
            // fetch all modified and added files
            foreach (var fileToDownload in prepareDownload)
            {
                var urlEncoded = HttpUtility.UrlEncode(fileToDownload);
                
                var url =
                    $"{GITLAB_PROJECT_URL}{@event.project_id}/repository/files/{urlEncoded}/?ref={@event.@ref}";
                
                Console.WriteLine($"Downloading: {fileToDownload} with url: {url} - {list.Count}/{prepareDownload.Count}");
                
                // Fetch the file
                var result = await httpClient.GetAsync(
                    url);

                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var file = JsonConvert.DeserializeObject<GitLabFile>(content);
                    list.Add(file);
                    Console.WriteLine($"File: {fileToDownload} successfully downloaded - {list.Count}/{prepareDownload.Count}");
                }
                else
                {
                    var content = await result.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to download {fileToDownload} - {list.Count}/{prepareDownload.Count} - Error Msg: {result.ReasonPhrase} - Description: {content}");
                }
            }
            
            Console.WriteLine("Commit successfully downloaded - Publishing GitLabCommitV1 message async");

            if (list.Count == 0)
            {
                Console.WriteLine("No files where downloaded do not send commit msg");
                return;
            }
            
            // Send message with rabbitMQ manager
            await _rabbitMqManager.PublishMessageAsync(new GitLabCommitV1
            {
                object_kind = @event.object_kind,
                before = @event.before,
                after = @event.after,
                checkout_sha = @event.checkout_sha,
                CommitedFiles = list,
                project = @event.project,
                project_id = @event.project_id,
                @ref = @event.@ref,
                repository = @event.repository,
                user_id = @event.user_id,
                user_email = @event.user_email
            });
        }
    }
}