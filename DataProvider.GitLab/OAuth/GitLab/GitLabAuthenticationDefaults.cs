namespace DataProvider.GitLab.OAuth.GitLab
{
    public class GitLabAuthenticationDefaults
    {
        public const string AuthenticationScheme = "GitLab";

        public static readonly string DisplayName = "GitLab";

        public const string ClaimsIssuer = "GitLab";

        public const string CallbackPath = "/account/signin-gitlab";

        public static readonly string AuthorizationEndpoint = "https://gitlab.com/oauth/authorize";

        public static readonly string TokenEndpoint = "https://gitlab.com/oauth/token";

        public static readonly string UserInformationEndpoint = "https://gitlab.com/api/v4/user";
    }
}