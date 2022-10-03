using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlackPrBot.Extensions
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> JsonPostAsync<T>(
           this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PostAsync(url, content);
        }

        public static async Task<T> JsonReadAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(dataAsString);
        }
    }
}
