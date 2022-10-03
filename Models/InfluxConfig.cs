namespace SlackPrBot.Models
{
    internal class InfluxConfig
    {
        public string Url { get; set; }
        public string BucketName { get; set; }
        public string OrganizationName { get; set; }
        public int RetentionSeconds { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
    }
}