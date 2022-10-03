using System;
using System.Collections.Generic;
using System.Linq;

using SlackPrBot.Extensions;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;
using SlackPrBot.Models.Slack.Model;

namespace SlackPrBot.Models.Slack.Api
{
    internal class SettingsList : ISlackMessage
    {
        public string Channel { get; set; }
        public bool ReplaceMessage { get; set; }
        public IReadOnlyCollection<SettingsEditInfo> Settings { get; set; }
        public string UserId { get; set; }

        public Message ToSlackMessage()
        {
            var blocks = new List<IBlock>();

            blocks.AddRange(new IBlock[]
                {
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = ":gear: Settings list"
                        }
                    },
                    new DividerBlock(),
            });

            var settings = RenderSettingsBlocks();


            if (settings.Any())
            {
                blocks.AddRange(settings);
            }

            return new Message
            {
                Channel = Channel,
                User = UserId,
                ReplaceOriginal = ReplaceMessage,
                Text = "Settings list",
                Blocks = blocks.ToArray()
            };
        }

        private IReadOnlyCollection<IBlock> RenderSettingsBlocks()
        {
            if (Settings == null)
            {
                return Array.Empty<IBlock>();
            }

            var result = new List<IBlock>();
            result.Add(
                new SectionBlock
                {
                    Text = new TextObject
                    {
                        Text = "*Settings:*"
                    }
                }
            );

            foreach (var settingsInfo in Settings)
            {
                result.Add(new SectionBlock
                {
                    Text = new TextObject
                    {
                        Text = RepoHelper.RepoPath(settingsInfo.IsGitlab, settingsInfo.BaseRepoPath, settingsInfo.Project, settingsInfo.Repo),
                    },
                    Accessory = new ActionElement
                    {
                        Type = ActionElement.OverflowType,
                        ActionId = "settings-list-settings",
                        Options = new[]
                        {
                            new SelectOption
                            {
                                Text = new PlainTextObject
                                {
                                    Text = "Edit"
                                },
                                Value = $"edit|{settingsInfo.Id}"
                            },
                            new SelectOption
                            {
                                Text = new PlainTextObject
                                {
                                    Text = "Delete"
                                },
                                Value = $"delete|{settingsInfo.Id}"
                            }
                        }
                    }
                });
            }

            return result;
        }
    }
}