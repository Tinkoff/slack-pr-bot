namespace SlackPrBot.Extensions
{
    public static class UserExtensions
    {
        public static string FormatUser(string userId, string alternativeName, bool useLink)
        {
            if (useLink)
            {
                return string.IsNullOrEmpty(userId) ? $"`{alternativeName}`" : $"<@{userId}>";
            }
            else
            {
                return $"`{alternativeName}`";
            }
        }
    }
}
