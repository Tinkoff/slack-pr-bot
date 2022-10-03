namespace SlackPrBot.Models.Slack.Api.Base.Blocks
{
    internal class DividerBlock : IBlock
    {
        public string BlockId { get; set; }
        public string Type => "divider";
    }
}
