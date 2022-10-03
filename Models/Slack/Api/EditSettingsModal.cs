using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.ModalViewBuilders;

namespace SlackPrBot.Models.Slack.Api
{
    internal class EditSettingsModal : ISlackEditModal
    {
        private readonly SettingsModalBuilder _settingsModalBuilder;

        public EditSettingsModal(SettingsModalBuilder settingsModalBuilder)
        {
            _settingsModalBuilder = settingsModalBuilder;
        }

        public EditModal ToSlackModalMessage()
        {
            var view = _settingsModalBuilder.Build();
            var id = view.Id;
            view.Id = null;

            return new EditModal
            {
                ViewId = id,
                View = view
            };
        }
    }
}
