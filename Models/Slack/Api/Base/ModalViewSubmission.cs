using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class ModalViewSubmission : ModalView
    {
        [JsonProperty("state")]
        public ViewInputState State { get; set; }

        public ValueInputResult GetInitialValue(string blockId) => GetInitialValue(blockId, blockId);

        public ValueInputResult GetInitialValue(string blockId, string actionId)
        {
            if (State.Values.TryGetValue(blockId, out var value))
            {
                if (value.TryGetValue(actionId, out var result))
                {
                    return result;
                }
            }

            return null;
        }
    }
}