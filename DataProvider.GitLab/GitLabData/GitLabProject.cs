using System;
using System.Collections.Generic;

namespace DataProvider.GitLab.GitLabData
{
    public class GitLabProject
    {
        public int id { get; set; }
        public object description { get; set; }
        public string default_branch { get; set; }
        public string visibility { get; set; }
        public string ssh_url_to_repo { get; set; }
        public string http_url_to_repo { get; set; }
        public string web_url { get; set; }
        public string readme_url { get; set; }
        public List<string> tag_list { get; set; }
        public Owner owner { get; set; }
        public string name { get; set; }
        public string name_with_namespace { get; set; }
        public string path { get; set; }
        public string path_with_namespace { get; set; }
        public bool issues_enabled { get; set; }
        public int open_issues_count { get; set; }
        public bool merge_requests_enabled { get; set; }
        public bool jobs_enabled { get; set; }
        public bool wiki_enabled { get; set; }
        public bool snippets_enabled { get; set; }
        public bool can_create_merge_request_in { get; set; }
        public bool resolve_outdated_diff_discussions { get; set; }
        public bool container_registry_enabled { get; set; }
        public DateTime created_at { get; set; }
        public DateTime last_activity_at { get; set; }
        public int creator_id { get; set; }
        public Namespace @namespace { get; set; }
        public string import_status { get; set; }
        public bool archived { get; set; }
        public string avatar_url { get; set; }
        public bool shared_runners_enabled { get; set; }
        public int forks_count { get; set; }
        public int star_count { get; set; }
        public string runners_token { get; set; }
        public int ci_default_git_depth { get; set; }
        public bool public_jobs { get; set; }
        public List<object> shared_with_groups { get; set; }
        public bool only_allow_merge_if_pipeline_succeeds { get; set; }
        public bool only_allow_merge_if_all_discussions_are_resolved { get; set; }
        public bool remove_source_branch_after_merge { get; set; }
        public bool request_access_enabled { get; set; }
        public string merge_method { get; set; }
        public bool autoclose_referenced_issues { get; set; }
        public object suggestion_commit_message { get; set; }
        public string marked_for_deletion_at { get; set; }
        public string marked_for_deletion_on { get; set; }
        public Statistics statistics { get; set; }
        public Links _links { get; set; }
    }
    public class Owner
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
    }

    public class Namespace
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string kind { get; set; }
        public string full_path { get; set; }
    }

    public class Statistics
    {
        public int commit_count { get; set; }
        public int storage_size { get; set; }
        public int repository_size { get; set; }
        public int wiki_size { get; set; }
        public int lfs_objects_size { get; set; }
        public int job_artifacts_size { get; set; }
        public int packages_size { get; set; }
    }

    public class Links
    {
        public string self { get; set; }
        public string issues { get; set; }
        public string merge_requests { get; set; }
        public string repo_branches { get; set; }
        public string labels { get; set; }
        public string events { get; set; }
        public string members { get; set; }
    }
}