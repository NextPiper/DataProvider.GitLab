using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace DataProvider.GitLab.OAuth.GitLab
{
    public static class GitLabAuthenticationExtensions
    {
        public static AuthenticationBuilder AddGitLab(this AuthenticationBuilder builder, Action<GitLabAuthenticationOptions> configuration)
        {
            return builder.AddOAuth<GitLabAuthenticationOptions, GitLabAuthenticationHandler>(GitLabAuthenticationDefaults.AuthenticationScheme,
                GitLabAuthenticationDefaults.DisplayName, configuration);
        }
    }
}