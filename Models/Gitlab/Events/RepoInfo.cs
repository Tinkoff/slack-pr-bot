using System;
using System.Linq;

using Newtonsoft.Json;

namespace SlackPrBot.Models.Gitlab.Events
{
    internal class RepoInfo
    {
        [JsonProperty("path_with_namespace")]
        public string PathWithNamespaces { get; set; }

        [JsonIgnore]
        public string Slug => PathWithNamespaces.Split('/').Last();

        [JsonIgnore]
        public string Key => PathWithNamespaces.Substring(0, PathWithNamespaces.LastIndexOf('/') > 0 ? PathWithNamespaces.LastIndexOf('/') : PathWithNamespaces.Length);

    }
}
