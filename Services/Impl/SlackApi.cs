using Microsoft.Extensions.Configuration;

using SlackPrBot.Extensions;
using SlackPrBot.Models;
using SlackPrBot.Models.Slack.Api;
using SlackPrBot.Models.Slack.Api.Base;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SlackPrBot.Services.Impl
{
    internal class SlackApi : IDisposable
    {
        private readonly HttpClient _client;
        private readonly HttpClientHandler _handler;

        public SlackApi(IConfiguration config)
        {
            var proxyCredentials = config.GetSection("ProxyCredentials").Get<Credentials>();
            if (!string.IsNullOrEmpty(proxyCredentials.Url))
            {
                NetworkCredential credentials = null;

                if (!string.IsNullOrEmpty(proxyCredentials.UserName) && !string.IsNullOrEmpty(proxyCredentials.Password))
                {
                    credentials = new NetworkCredential(proxyCredentials.UserName, proxyCredentials.Password, proxyCredentials.Domain);
                }

                _handler = new HttpClientHandler
                {
                    UseProxy = true,
                    Proxy = new WebProxy(new Uri(proxyCredentials.Url), false, Array.Empty<string>(), credentials)
                };

                _client = new HttpClient(_handler);
            }
            else
            {
                _client = new HttpClient();
            }

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.GetValue<string>("BotSlackKey"));
        }

        public void Dispose()
        {
            _handler?.Dispose();
            _client?.Dispose();
        }

        public async Task EditModalAsync(ISlackEditModal modal)
        {
            await PostAsync<BaseResult, EditModal>("https://slack.com/api/views.update", modal.ToSlackModalMessage());
            return;
        }

        public async Task<Message> FindMessageByTimestampAsync(string channel, string ts)
        {
            var result = await GetAsync<HistoryResult>($"https://slack.com/api/conversations.history?channel={channel}&latest={ts}&inclusive=true&limit=1");
            return result.Messages.Any() ? result.Messages[0] : null;
        }

        public async Task<string> FindUserIdByEmailAsync(string email)
        {
            var result = await GetAsync<UserResult>($"https://slack.com/api/users.lookupByEmail?email={email}");
            return result.User.Id;
        }

        public async Task<UserGroup[]> GetUserGroupsAsync()
        {
            var result = await GetAsync<UserGroupsResult>("https://slack.com/api/usergroups.list?include_count=false&include_disabled=false&include_users=false");
            return result.UserGroups;
        }

        public async Task<Channel[]> GetBotChannelsAsync()
        {
            var result = await GetAsync<UserConversationResult>("https://slack.com/api/users.conversations?types=public_channel,private_channel,mpim,im");
            return result.Channels;
        }

        public async Task OpenModalAsync(OpenModal modal)
        {
            await PostAsync<BaseResult, OpenModal>("https://slack.com/api/views.open", modal);
            return;
        }

        public async Task<string> PostEphemeralMessageAsync(ISlackMessage message)
        {
            var result = await PostAsync<PostMessageResult, Message>("https://slack.com/api/chat.postEphemeral", message.ToSlackMessage());
            return result.Ts;
        }

        public async Task<string> PostMessageAsync(ISlackMessage message)
        {
            var slackMessage = message.ToSlackMessage();
            slackMessage.AsUser = true;
            var result = await PostAsync<PostMessageResult, Message>("https://slack.com/api/chat.postMessage", slackMessage);
            return result.Ts;
        }

        public Task PostResponseAsync(string url, ISlackMessage message) => PostAsync(url, message.ToSlackMessage());

        public async Task<string> UpdateMessageAsync(ISlackMessage message)
        {
            var slackMessage = message.ToSlackMessage();
            slackMessage.AsUser = true;
            var result = await PostAsync<PostMessageResult, Message>("https://slack.com/api/chat.update", slackMessage);
            return result.Ts;
        }

        private async Task<TResponse> GetAsync<TResponse>(string url) where TResponse : BaseResult
        {
            var result = await _client.GetAsync(url);
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.JsonReadAsync<TResponse>();
                if (response.Ok)
                {
                    return response;
                }
                else
                {
                    throw new Exception(response.Error);
                }
            }
            else
            {
                throw new Exception($"{result.StatusCode} {result.ReasonPhrase}");
            }
        }

        private async Task PostAsync<TRequest>(string url, TRequest request)
        {
            var result = await _client.JsonPostAsync(url, request);
            if (result.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                throw new Exception($"{result.StatusCode} {result.ReasonPhrase}");
            }
        }

        private async Task<TResponse> PostAsync<TResponse, TRequest>(string url, TRequest request) where TResponse : BaseResult
        {
            var result = await _client.JsonPostAsync(url, request);
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.JsonReadAsync<TResponse>();
                if (response.Ok)
                {
                    return response;
                }
                else
                {
                    throw new Exception(response.Error);
                }
            }
            else
            {
                throw new Exception($"{result.StatusCode} {result.ReasonPhrase}");
            }
        }
    }
}