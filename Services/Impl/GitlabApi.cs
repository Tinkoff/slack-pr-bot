using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using SlackPrBot.Extensions;
using SlackPrBot.Models.Gitlab;

namespace SlackPrBot.Services.Impl
{
    internal class GitlabApi : IDisposable
    {
        private readonly HttpClient _client;

        public GitlabApi()
        {
            _client = new HttpClient();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public Task<MergeRequestResponse> GetMergeRequestAsync(string basePath, string token, string projectPath, int mergeRequistIid)
        {
            return GetAsync<MergeRequestResponse>($"{GenerateBaseUrl(basePath)}/projects/{Uri.EscapeDataString(projectPath)}/merge_requests/{mergeRequistIid}?private_token={token}");
        }

        private async Task<TResponse> GetAsync<TResponse>(string url)
        {
            var result = await _client.GetAsync(url);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.JsonReadAsync<TResponse>();
            }
            else
            {
                throw new Exception($"{result.StatusCode} {result.ReasonPhrase}");
            }
        }

        private static string GenerateBaseUrl(string basePath)
        {
            return $"{basePath}/api/v4";
        }
    }
}