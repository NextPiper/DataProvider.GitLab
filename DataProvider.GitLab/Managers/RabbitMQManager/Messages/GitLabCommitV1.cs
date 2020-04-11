using System.Collections.Generic;
using DataProvider.GitLab.GitLabData;

namespace DataProvider.GitLab.Managers.RabbitMQManager.Messages
{
    public class GitLabCommitV1
    {
        public string object_kind { get; set; }
        public string before { get; set; }
        public string after { get; set; }
        public string @ref { get; set; }
        public string checkout_sha { get; set; }
        public int user_id { get; set; }
        public string user_email { get; set; }
        public int project_id { get; set; }
        public Project project { get; set; }
        public Repository repository { get; set; }
        public IEnumerable<GitLabFile> CommitedFiles { get; set; }
    }
}