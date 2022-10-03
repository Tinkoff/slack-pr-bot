
using Atlassian.Jira;

using Microsoft.Extensions.Configuration;

using RestSharp;

using SlackPrBot.Models;

namespace SlackPrBot.Services.Impl
{
    internal class JiraInternalService
    {
        private readonly Credentials _atlassianCredentials;

        public JiraInternalService(IConfiguration config)
        {
            _atlassianCredentials = config.GetSection("AtlassianCredentials").Get<Credentials>();

            Client = CreateRestClient();
        }

        public Jira Client { get; }

        private Jira CreateRestClient()
        {
            if (string.IsNullOrEmpty(_atlassianCredentials.Url))
            {
                return null;
            }

            return Jira.CreateRestClient(_atlassianCredentials.Url, _atlassianCredentials.UserName, _atlassianCredentials.Password);
        }
    }
}
