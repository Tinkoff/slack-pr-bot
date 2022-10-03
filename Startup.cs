using System.Linq;

using InfluxDB.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using SlackPrBot.DomainModels;
using SlackPrBot.DomainModels.Entities;
using SlackPrBot.HostedServices;
using SlackPrBot.JsonConverters;
using SlackPrBot.Models;
using SlackPrBot.Services;
using SlackPrBot.Services.Impl;

namespace SlackPrBot
{
    public class Startup
    {
        public Startup()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new BlockConverter());

            JsonConvert.DefaultSettings = () => settings;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/bot-{Date}.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            WarmupEf(app);
            CreateBucket(app);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<StaleNotification>();

            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.Converters.Add(new BlockConverter()));

            services.AddScoped<Context>();

            services.AddScoped<IStashEventService, StashEventService>();
            services.AddScoped<IGitlabEventService, GitlabEventService>();
            services.AddScoped<PullRequestService>();
            services.AddScoped<PushService>();
            services.AddScoped<CommentService>();
            services.AddScoped<SlackApi>();
            services.AddScoped<GitlabApi>();
            services.AddScoped<SettingsService>();
            services.AddScoped<UserService>();
            services.AddScoped<JiraWorkflowService>();
            services.AddScoped<JiraInternalService>();

            services.AddScoped<NotificationService>();

            services.AddScoped<ISlackSlashService, SlackSlashService>();
            services.AddScoped<SetupSlashCommandService>();

            services.AddScoped<ISlackInteractiveService, SlackInteractiveService>();
            services.AddScoped<SetupInteractiveService>();
            services.AddScoped<JiraInteractiveService>();

            services.AddScoped<StatisticsSlashCommandService>();

            services.AddScoped<GitlabMergeRequestEventsConverter>();
            services.AddScoped<GitlabNoteConverter>();

            services.AddSingleton<Cache>();

            services.AddScoped<StatisticService>();
        }

        private void WarmupEf(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<Context>();
            context.Set<PullRequest>().Any();
        }

        private void CreateBucket(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var statisticService = scope.ServiceProvider.GetService<StatisticService>();
            statisticService.CreateBucketAsync().Wait();
        }
    }
}