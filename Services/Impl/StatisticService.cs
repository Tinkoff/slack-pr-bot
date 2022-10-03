using System;
using System.Linq;
using System.Threading.Tasks;

using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SlackPrBot.DomainModels.Entities;
using SlackPrBot.Models;

namespace SlackPrBot.Services.Impl
{
    internal class StatisticService
    {
        private readonly InfluxConfig _influxConfig;
        private readonly ILogger<StatisticService> _log;

        public StatisticService(IConfiguration configuration, ILogger<StatisticService> log)
        {
            _influxConfig = configuration?.GetSection("Influx")?.Get<InfluxConfig>();
            _log = log;
        }

        public async Task CreateBucketAsync()
        {
            using var client = CreateClient();
            if (client == null)
            {
                return;
            }

            var bucketsApi = client.GetBucketsApi();

            var bucket = await bucketsApi.FindBucketByNameAsync(_influxConfig.BucketName);
            if (bucket != null)
            {
                return;
            }

            var organizationsApi = client.GetOrganizationsApi();

            var organization = (await organizationsApi.FindOrganizationsAsync()).FirstOrDefault(x => x.Name == _influxConfig.OrganizationName);
            if (organization == null)
            {
                organization = await organizationsApi.CreateOrganizationAsync(_influxConfig.OrganizationName);
            }

            await bucketsApi.CreateBucketAsync(_influxConfig.BucketName, new BucketRetentionRules(BucketRetentionRules.TypeEnum.Expire, _influxConfig.RetentionSeconds), organization);
        }


        public async Task PullRequestCreatedAsync(PullRequest pull, string author)
        {
            using var client = CreateClient();
            if (client == null)
            {
                return;
            }

            try
            {
                var writeApi = client.GetWriteApiAsync();
                var point = PointData.Measurement("pull_created")
                    .Tag("project", pull.ProjectName)
                    .Tag("repo", pull.RepoName)
                    .Tag("author", author)
                    .Tag("pull_id", (pull.Iid ?? pull.PullId).ToString())
                     .Tag("jira", pull.TaskId ?? "")
                    .Field("created", true)
                    .Timestamp(DateTime.UtcNow, WritePrecision.Ms);

                await writeApi.WritePointAsync(_influxConfig.BucketName, _influxConfig.OrganizationName, point);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while sending data to stat");
            }
        }

        public async Task PullRequestApprovedAsync(PullRequest pull, string approver, string author)
        {
            using var client = CreateClient();
            if (client == null)
            {
                return;
            }

            try
            {
                var writeApi = client.GetWriteApiAsync();
                var point = PointData.Measurement("pull_approved")
                    .Tag("project", pull.ProjectName)
                    .Tag("repo", pull.RepoName)
                    .Tag("author", author)
                    .Tag("approver", approver)
                    .Tag("pull_id", (pull.Iid ?? pull.PullId).ToString())
                     .Tag("jira", pull.TaskId ?? "")
                    .Tag("approved", "true")
                    .Field("since_created_sec", (DateTime.Now - pull.Created).TotalSeconds)
                    .Timestamp(DateTime.UtcNow, WritePrecision.Ms);


                await writeApi.WritePointAsync(_influxConfig.BucketName, _influxConfig.OrganizationName, point);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while sending data to stat");
            }
        }

        public async Task PullRequestDeclinedAsync(PullRequest pull, string approver, string author)
        {
            using var client = CreateClient();
            if (client == null)
            {
                return;
            }

            try
            {
                var writeApi = client.GetWriteApiAsync();
                var point = PointData.Measurement("pull_approved")
                    .Tag("project", pull.ProjectName)
                    .Tag("repo", pull.RepoName)
                    .Tag("author", author)
                    .Tag("approver", approver)
                    .Tag("approved", "false")
                    .Tag("pull_id", (pull.Iid ?? pull.PullId).ToString())
                     .Tag("jira", pull.TaskId ?? "")
                    .Field("since_created_sec", (DateTime.Now - pull.Created).TotalSeconds)
                    .Timestamp(DateTime.UtcNow, WritePrecision.Ms);



                await writeApi.WritePointAsync(_influxConfig.BucketName, _influxConfig.OrganizationName, point);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while sending data to stat");
            }
        }

        public async Task PullRequestClosedAsync(PullRequest pull, string closer, string author)
        {
            using var client = CreateClient();
            if (client == null)
            {
                return;
            }

            try
            {
                var writeApi = client.GetWriteApiAsync();
                var point = PointData.Measurement("pull_closed")
                    .Tag("project", pull.ProjectName)
                    .Tag("repo", pull.RepoName)
                    .Tag("author", author)
                    .Tag("closer", closer)
                    .Tag("pull_id", (pull.Iid ?? pull.PullId).ToString())
                     .Tag("jira", pull.TaskId ?? "")
                    .Field("since_created_sec", (DateTime.Now - pull.Created).TotalSeconds)
                    .Timestamp(DateTime.UtcNow, WritePrecision.Ms);


                await writeApi.WritePointAsync(_influxConfig.BucketName, _influxConfig.OrganizationName, point);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while sending data to stat");
            }
        }

        public async Task PullRequestMergedAsync(PullRequest pull, string merger, string author)
        {
            using var client = CreateClient();
            if (client == null)
            {
                return;
            }

            try
            {
                var writeApi = client.GetWriteApiAsync();
                var point = PointData.Measurement("pull_merged")
                    .Tag("project", pull.ProjectName)
                    .Tag("repo", pull.RepoName)
                    .Tag("author", author)
                    .Tag("merger", merger)
                    .Tag("pull_id", (pull.Iid ?? pull.PullId).ToString())
                    .Tag("jira", pull.TaskId ?? "")
                    .Field("since_created_sec", (DateTime.Now - pull.Created).TotalSeconds)
                    .Timestamp(DateTime.UtcNow, WritePrecision.Ms);


                await writeApi.WritePointAsync(_influxConfig.BucketName, _influxConfig.OrganizationName, point);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while sending data to stat");
            }
        }

        private InfluxDBClient CreateClient()
        {
            if (_influxConfig == null || string.IsNullOrEmpty(_influxConfig.Url))
            {
                return null;
            }

            return InfluxDBClientFactory.Create(_influxConfig.Url, _influxConfig.UserName, _influxConfig.UserPassword.ToCharArray());
        }
    }
}