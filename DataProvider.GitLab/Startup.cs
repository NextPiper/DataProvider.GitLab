using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using DataProvider.GitLab.Managers.GitLabHandler;
using DataProvider.GitLab.Managers.RabbitMQManager;
using DataProvider.GitLab.OAuth.GitLab;
using DataProvider.GitLab.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

namespace DataProvider.GitLab
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "NextPipe Beta Core API", Version = "v1"});
            });
            
            services.AddScoped<IGitLabHandler, GitLabHandler>();
            services.AddScoped<IRabbitMQManager, RabbitMQManager>();

            // Configure GitHub OAuth
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = IdentityConstants.ExternalScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                    options.DefaultChallengeScheme = "GitLab";
                })
                .AddCookie(IdentityConstants.ExternalScheme)
                .AddGitLab(options =>
                {
                    options.ClientId = Configuration["GitLab:ClientId"];
                    options.ClientSecret = Configuration["GitLab:ClientSecret"];

                    options.SaveTokens = true;
                    options.Events.OnCreatingTicket = ctx =>
                    {
                        var tokens = ctx.Properties.GetTokens() as List<AuthenticationToken>;
                        tokens.Add(new AuthenticationToken()
                        {
                            Name = "TicketCreated",
                            Value = DateTime.UtcNow.ToString()
                        });
                        ctx.Properties.StoreTokens(tokens);
                        return Task.CompletedTask;
                    };
                });


            /*.AddOAuth("GitHub", options =>
            {
                options.ClientId = Configuration["GitHub:ClientId"];
                options.ClientSecret = Configuration["GitHub:ClientSecret"];
                options.CallbackPath = new PathString("/account/signin-github");

                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";
                
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey("urn:github:login", "login");
                options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
                options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

                options.SaveTokens = true;

                options.Events.OnCreatingTicket = async context =>
                {
                    Console.WriteLine("Creating ticket");
                    var request =
                        new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", context.AccessToken);

                    var response = await context.Backchannel.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                    response.EnsureSuccessStatusCode();

                    var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                    context.RunClaimActions(user.RootElement);
                };

            })
            .AddOAuth("GitLab","GitLab", options =>
            {
                options.ClientId = Configuration["GitLab:ClientId"];
                options.ClientSecret = Configuration["GitLab:ClientSecret"];
                
                options.ClaimsIssuer = "GitLab";
                options.CallbackPath = new PathString("/account/signin-gitlab");

                options.AuthorizationEndpoint = "https://gitlab.com/oauth/authorize";
                options.TokenEndpoint = "https://gitlab.com/oauth/token";
                options.UserInformationEndpoint = "https://gitlab.com/api/v4/user";

                options.SaveTokens = true;

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request =
                            new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization =
                            new AuthenticationHeaderValue("bearer", context.AccessToken);
                        var response = await context.Backchannel.SendAsync(request,
                            HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                        context.RunClaimActions(user.RootElement);
                    }
                };
            });*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}