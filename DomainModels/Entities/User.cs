using System;


namespace SlackPrBot.DomainModels.Entities
{
    internal class User
    {
        public string Email { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SlackUserId { get; set; }

        public string Login => Email.Split('@')[0];
    }
}
