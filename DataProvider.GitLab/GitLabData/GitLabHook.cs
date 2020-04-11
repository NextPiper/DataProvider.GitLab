using System;

namespace DataProvider.GitLab.GitLabData
{
    public class GitLabHook
    {
        public int id { get; set; }
        public string url { get; set; }
        public int project_id { get; set; }
        public bool push_events { get; set; }
        public string push_events_branch_filter { get; set; }
        public bool issues_events { get; set; }
        public bool confidential_issues_events { get; set; }
        public bool merge_requests_events { get; set; }
        public bool tag_push_events { get; set; }
        public bool note_events { get; set; }
        public bool job_events { get; set; }
        public bool pipeline_events { get; set; }
        public bool wiki_page_events { get; set; }
        public bool enable_ssl_verification { get; set; }
        public DateTime created_at { get; set; }
    }
}