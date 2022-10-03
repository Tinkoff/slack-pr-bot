using System;
using System.Collections.Generic;
using System.Linq;

using SlackPrBot.DomainModels.Entities;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api.ModalViewBuilders
{
    internal class SettingsModalBuilder
    {
        private readonly ModalViewSubmission _resultView;
        private string _channel;
        private string _dbId;
        private Dictionary<string, string> _initialValues = new Dictionary<string, string>();
        private string _title = "New integration";
        private bool _toogleJira;
        private bool _toogleGitlab;

        public SettingsModalBuilder(ModalViewSubmission resultView = null)
        {
            _resultView = resultView;
        }

        public SettingsModalBuilder AddDbId(string id)
        {
            _dbId = id;
            return this;
        }

        public SettingsModalBuilder AddValue(string name, string value)
        {
            _initialValues.Add(name, value);
            return this;
        }

        public ModalView Build()
        {
            var blocks = new List<IBlock>();
            var gitlabToggle = new IBlock[]
                {
                    new ActionBlock
                    {
                        BlockId = "settings-gitlabOnOff",
                        Elements = new[]
                        {
                            ToogleGitlabTemplate()
                        }
                    }
                };

            blocks.AddRange(gitlabToggle);
            blocks.AddRange(RenderGitlabBlocks());
            blocks.Add(new InputBlock
            {
                BlockId = nameof(Settings.RepoUrl),
                Label = new PlainTextObject
                {
                    Text = "Repo URL"
                },
                Hint = new PlainTextObject
                {
                    Text = "Set your repo url, for example https://gitlab.company.ru"
                },
                Element = new Element
                {
                    Type = Element.TextInput,
                    InitialValue = GetInitialValue(nameof(Settings.RepoUrl)),
                    ActionId = nameof(Settings.RepoUrl),
                    Placeholder = new PlainTextObject
                    {
                        Text = " "
                    },
                }
            });

            var firstBlocks = new IBlock[]
                {
                    new InputBlock
                    {
                        BlockId = nameof(Settings.Project),
                        Label = new PlainTextObject
                        {
                            Text = "Project"
                        },
                        Hint = new PlainTextObject
                        {
                            Text = "For example: https://stash_url/projects/{PROJECT}/repos or https//gitlab_url/{group/subgroup}"
                        },
                        Element = new Element
                        {
                            InitialValue = GetInitialValue(nameof(Settings.Project)),
                            Type = Element.TextInput,
                            ActionId = nameof(Settings.Project),
                            Placeholder = new PlainTextObject
                            {
                                Text = " "
                            }
                        }
                    },
                    new InputBlock
                        {
                        BlockId = nameof(Settings.Repo),
                        Label = new PlainTextObject
                        {
                            Text = "Repo"
                        },
                        Hint = new PlainTextObject
                        {
                            Text = "For example: https://stash_url/projects/PROJECT/repos/{REPO} or https//gitlab_url/group/subgroup/{repo}"
                        },
                        Element = new Element
                        {
                            InitialValue = GetInitialValue(nameof(Settings.Repo)),
                            Type = Element.TextInput,
                            ActionId = nameof(Settings.Repo),
                            Placeholder = new PlainTextObject
                            {
                                Text = " "
                            }
                        }
                    },
                    new InputBlock
                    {
                        BlockId = nameof(Settings.StaleTime),
                        Optional = true,
                        Label = new PlainTextObject
                        {
                            Text = "Stale time"
                        },
                        Element = new Element
                        {
                            Type = Element.TextInput,
                            InitialValue = GetInitialValue(nameof(Settings.StaleTime)),
                            ActionId = nameof(Settings.StaleTime),
                            Placeholder = new PlainTextObject
                            {
                                Text = "1.00:00:00"
                            }
                        }
                    },
                    new InputBlock
                    {
                        BlockId = nameof(Settings.ExcludeToBranches),
                        Optional = true,
                        Label = new PlainTextObject
                        {
                            Text = "Exclude prs merged into"
                        },
                        Hint = new PlainTextObject
                        {
                            Text = "type comma separated branches."
                        },
                        Element = new Element
                        {
                            Type = Element.TextInput,
                            InitialValue = GetInitialValue(nameof(Settings.ExcludeToBranches)),
                            ActionId = nameof(Settings.ExcludeToBranches),
                            Placeholder = new PlainTextObject
                            {
                                Text = "branch1,branch2"
                            }
                        }
                    },
                    new InputBlock
                    {
                        BlockId = nameof(Settings.NotifyUsers),
                        Optional = true,
                        Label = new PlainTextObject
                        {
                            Text = "Notify users or groups on new pr"
                        },
                        Hint = new PlainTextObject
                        {
                            Text = "type comma separated logins or groupnames."
                        },
                        Element = new Element
                        {
                            Type = Element.TextInput,
                            InitialValue = GetInitialValue(nameof(Settings.NotifyUsers)),
                            ActionId = nameof(Settings.NotifyUsers),
                            Placeholder = new PlainTextObject
                            {
                                Text = "user@example.ru,test_group"
                            }
                        }
                    },

                    new ActionBlock
                    {
                        BlockId = "settings-jiraOnOff",
                        Elements = new[]
                        {
                            ToogleJiraTemplate()
                        }
                    },
                };

            blocks.AddRange(firstBlocks);
            blocks.AddRange(RenderJiraBlocks());

            return new ModalView
            {
                Id = _resultView?.Id,
                ClearOnClose = true,
                CallbackId = ModalCallbackId.CreateSettings.ToString(),
                PrivateMetadata = GetChannel(),
                Title = new PlainTextObject
                {
                    Text = _resultView?.Title?.Text ?? _title
                },
                Close = new PlainTextObject
                {
                    Text = "Close"
                },
                Submit = new PlainTextObject
                {
                    Text = "Save"
                },
                Blocks = blocks.ToArray()
            };
        }

        public SettingsModalBuilder SetChannel(string channel)
        {
            _channel = channel;

            return this;
        }

        public SettingsModalBuilder SetTitle(string title)
        {
            _title = title;
            return this;
        }

        public SettingsModalBuilder ToggleJira()
        {
            _toogleJira = true;
            return this;
        }

        public SettingsModalBuilder ToggleGitlab()
        {
            _toogleGitlab = true;
            return this;
        }

        private string GetChannel()
        {
            if (_resultView != null)
            {
                return _resultView.PrivateMetadata;
            }

            var result = _channel;
            if (!string.IsNullOrEmpty(_dbId))
            {
                result += $"|{_dbId}";
            }

            return result;
        }

        private string GetInitialValue(string blockId) => GetInitialValue(blockId, blockId);

        private string GetInitialValue(string blockId, string actionId)
        {
            if (_resultView != null)
            {
                return _resultView.GetInitialValue(blockId, actionId)?.Value;
            }

            if (_initialValues.TryGetValue(blockId, out var value))
            {
                return value;
            }

            return null;
        }

        private bool IsJiraEnabled()
        {
            if (_initialValues.TryGetValue(nameof(Settings.JiraSupport), out var value))
            {
                return bool.Parse(value);
            }

            var block = _resultView?.Blocks.First(x => x.BlockId == "settings-jiraOnOff");
            var oldValue = block != null ? bool.Parse((block as ActionBlock).Elements.First().Value) : true;
            return _toogleJira ? !oldValue : oldValue;
        }

        private IBlock[] RenderJiraBlocks()
        {
            var isJiraEnabled = IsJiraEnabled();

            if (!isJiraEnabled)
            {
                return Array.Empty<IBlock>();
            }

            return new[]
            {
                new InputBlock
                {
                    BlockId = nameof(Settings.DevelopmentStatuses),
                    Label = new PlainTextObject
                    {
                        Text = "Development statuses"
                    },
                    Hint = new PlainTextObject
                    {
                        Text = "Every task type should be on new line. format: task type|from-status|transition-name"
                    },
                    Element = new Element
                    {
                        Type = Element.TextInput,
                        Multiline = true,
                        InitialValue = GetInitialValue(nameof(Settings.DevelopmentStatuses)),
                        ActionId = nameof(Settings.DevelopmentStatuses),
                        Placeholder = new PlainTextObject
                        {
                            Text = " "
                        },
                    }
                },
                new InputBlock
                {
                    BlockId = nameof(Settings.ReadyToReviewStatuses),
                    Label = new PlainTextObject
                    {
                        Text = "Ready to review statuses"
                    },
                    Hint = new PlainTextObject
                    {
                        Text = "Every task type should be on new line. format: task type|from-status|transition-name"
                    },
                    Element = new Element
                    {
                        Type = Element.TextInput,
                        Multiline = true,
                        InitialValue = GetInitialValue(nameof(Settings.ReadyToReviewStatuses)),
                        ActionId = nameof(Settings.ReadyToReviewStatuses),
                        Placeholder = new PlainTextObject
                        {
                            Text = " "
                        }
                    }
                },
                new InputBlock
                {
                    BlockId = nameof(Settings.ReviewStatuses),
                    Label = new PlainTextObject
                    {
                        Text = "Review statuses"
                    },
                    Hint = new PlainTextObject
                    {
                        Text = "Every task type should be on new line. format: task type|from-status|transition-name"
                    },
                    Element = new Element
                    {
                        Type = Element.TextInput,
                        Multiline = true,
                        InitialValue = GetInitialValue(nameof(Settings.ReviewStatuses)),
                        ActionId = nameof(Settings.ReviewStatuses),
                        Placeholder = new PlainTextObject
                        {
                            Text = " "
                        }
                    }
                },
                new InputBlock
                {
                    BlockId = nameof(Settings.ReadyStatuses),
                    Label = new PlainTextObject
                    {
                        Text = "Ready statuses"
                    },
                    Hint = new PlainTextObject
                    {
                        Text = "Every task type should be on new line. format: task type|from-status|transition-name"
                    },
                    Element = new Element
                    {
                        Type = Element.TextInput,
                        Multiline = true,
                        InitialValue = GetInitialValue(nameof(Settings.ReadyStatuses)),
                        ActionId = nameof(Settings.ReadyStatuses),
                        Placeholder = new PlainTextObject
                        {
                            Text = " "
                        }
                    }
                },
                new InputBlock
                {
                    BlockId = nameof(Settings.ClosedStatuses),
                    Optional = true,
                    Label = new PlainTextObject
                    {
                        Text = "Closed statuses"
                    },
                    Hint = new PlainTextObject
                    {
                        Text = "Every task type should be on new line. format: task type|from-status|transition-name"
                    },
                    Element = new Element
                    {
                        Type = Element.TextInput,
                        Multiline = true,
                        InitialValue = GetInitialValue(nameof(Settings.ClosedStatuses)),
                        ActionId = nameof(Settings.ClosedStatuses),
                        Placeholder = new PlainTextObject
                        {
                            Text = " "
                        }
                    }
                },
            };
        }

        private ActionElement ToogleJiraTemplate()
        {
            var newValue = IsJiraEnabled();

            return new ActionElement
            {
                Type = ActionElement.ButtonType,
                ActionId = "settings-jiraOnOff",
                Style = newValue ? ActionElement.StyleDanger : null,
                Text = new PlainTextObject
                {
                    Text = newValue ? "Disable jira support" : "Enable jira support"
                },
                Value = newValue.ToString()
            };
        }

        private ActionElement ToogleGitlabTemplate()
        {
            var newValue = IsGitlabEnabled();

            return new ActionElement
            {
                Type = ActionElement.ButtonType,
                ActionId = "settings-gitlabOnOff",
                Style = newValue ? ActionElement.StyleDanger : null,
                Text = new PlainTextObject
                {
                    Text = newValue ? "Disable gitlab support" : "Enable gitlab support"
                },
                Value = newValue.ToString()
            };
        }

        private bool IsGitlabEnabled()
        {
            if (_initialValues.TryGetValue(nameof(Settings.IsGitlab), out var value))
            {
                return bool.Parse(value);
            }

            var block = _resultView?.Blocks.First(x => x.BlockId == "settings-gitlabOnOff");
            var oldValue = block != null ? bool.Parse((block as ActionBlock).Elements.First().Value) : false;
            return _toogleGitlab ? !oldValue : oldValue;
        }

        private IBlock[] RenderGitlabBlocks()
        {
            var isGitlabEnabled = IsGitlabEnabled();

            if (!isGitlabEnabled)
            {
                return Array.Empty<IBlock>();
            }

            return new[]
            {
                new InputBlock
                {
                    BlockId = nameof(Settings.GitlabToken),
                    Optional = true,
                    Label = new PlainTextObject
                    {
                        Text = "Gitlab token"
                    },
                    Hint = new PlainTextObject
                    {
                        Text = "Set your gitlab token. Bot could use it for clearing db from stale entries. This parameter optional."
                    },
                    Element = new Element
                    {
                        Type = Element.TextInput,
                        InitialValue = GetInitialValue(nameof(Settings.GitlabToken)),
                        ActionId = nameof(Settings.GitlabToken),
                        Placeholder = new PlainTextObject
                        {
                            Text = " "
                        },
                    }
                }
            };
        }
    }
}