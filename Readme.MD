
# Slack pull request bot
Bot can receive events from Stash or Gitlab and output them as slack messages. Using slack commands, you can check currently open or ready for merge pool requests. It is possible to integrate with JIRA Server so that bot can change task status within the life cycle of a pull request. The structure of the notifications looks like a mutable main message that displays the main information, and all additional messages, such as comments and other events, are written in a thread. Data is stored in SQLite.
## Install
To install, you need to create bot in slack and link it with application. Application runs under .NET 5.0 and requires ASP.NET Core Runtime. You can download the runtime from here: https://dotnet.microsoft.com/en-us/download/dotnet/5.0. You can run bot under IIS or under nginx.
### Slack setup
You need to create an application in slack and install it in the workspace. Configuration manifest example:
```
display_information:
  name: pr-bot
  description: pr-bot description
  background_color: "#5978d4"
features:
  bot_user:
    display_name: pr-bot
    always_online: true
  slash_commands:
    - command: /pr-bot
      url: https://{boturl}/slack/slash
      description: PR bot commands
      usage_hint: "[setup] [stat]"
      should_escape: false
oauth_config:
  scopes:
    bot:
      - channels:history
      - channels:read
      - chat:write
      - commands
      - groups:history
      - groups:read
      - im:history
      - im:read
      - mpim:history
      - mpim:read
      - users:read
      - users:read.email
      - usergroups:read
settings:
  interactivity:
    is_enabled: true
    request_url: https://{boturl}/slack/interactive
  org_deploy_enabled: false
  socket_mode_enabled: false
  token_rotation_enabled: false
 ```

You need to register oauth token in the BotSlackKey parameter in appsetting.json  To configure from the Slack side, you need to create a private or public channel and add bot user there.

Use **/pr-bot setup [add]** command to set up integration with stash or gitlab. If Jira is not needed then click **disable Jira support**

**Repo URL** - repository url

**Project** - project name in uppercase for stash or group name for gitlab

**Repo** - repository name

**Stale time** - optional parameter, when it is set, notifications will be sent to the channel that nothing has been done with the pool for a long time. The first parameter is days. It is optional if you specify a time less than a day.

**Exclude prs merged into** - optional parameter, you can specify which branches bot will ignore, branches are specified separated by commas, you need to specify target branch

**Notify users or groups on new pr** - tag groups or users when new pull request arrived

**/pr-bot setup add** add new integration

If integration already exists, then **/pr-bot setup** allows you to edit or remove integrations in this channel.
### Stash setup
In order for the bot to send notifications to Slack, you need to set up a webhook for the repository.  

URL: https://{boturl}/stash. **Opened, Declined, Deleted, Merged** events required, you can choose other events such as comments or push if you want more detailed notifications.
### gitlab setup
To integrate with gitlab, you need to enable **Enable gitlab support** button in the slack bot settings. Specify base path to your gitlab and user token that can read pool requests, token is not required, it is needed for clearing staled entries in db. From the gitlab side, you need to open access to the bot and if you specify token, then from the bot to the gitlab. In webhooks, specify the url https://{boturl}/gitlab and check **Merge request events** and, if necessary, **Comments**

Currently, gitlab does not have detailed events like stash. bot can respond to:
- creating a pool
- merge
- close
- change information on the pool (branch, description, name)
- adding a comment
- adding a commit
- approve
- withdrawal of approval
### bot configuration params
Bot settings are stored in the appsettings.json file
```
"BotSlackKey":  "", // slack oauth key
"StaleTimerCheck":  "00:00:00", // global check pool events timer
"ProxyCredentials": { // proxy settings
"Url":  "",
"UserName":  "",
"Password":  "",
"Domain":  ""
},
"AtlassianCredentials": { // Jira settings
"Url":  "",
"UserName":  "",
"Password":  "",
"Domain":  ""
},
"Influx": { // InfluxDB settings
"Url":  "",
"BucketName":  "statistic",
"OrganizationName":  "slack-pr-bot",
"UserName":  "",
"UserPassword":  "",
"RetentionSeconds":  63072000
}
```
## Jira integration
Integration only works with JIRA Server. With this integration, you can automatically change tasks statuses and send notifications as part of the pull request life cycle.

For this feature to work, you need user in Jira who has rights to change status. The base transition implementation is in *Services\Impl\JiraWorkflowService.cs*.

After adding user to Jira project, you need to set up status transitions in Slack.

Transition format:

- **Feature|develop|ready for review**
- **type|status from|transition name**

You need to specify each new transition from a new line. Bot have 5 status change events.
## Statistics
There are several commands that can be executed in a channel with integration or in a personal channel with a bot:

-   **/pr-bot stat open** - When sending a message to a channel, all open pull requests will be displayed. When sending a message to a bot channel, only my open pull requests will be displayed
-   **/pr-bot stat open my** - When sending a message to a channel, only my open pull requests will be displayed
-   **/pr-bot stat rtm** - When sending a message to a channel, all ready for merge pool requests will be displayed. When sending a message to a bot channel, only my ready for merge pool requests will be displayed
-   **/pr-bot stat rtm my** - When sending a message to a channel, only my ready for merge pool requests will be displayed

You can also enable InfluxDB statistics collection. A new entry is added for each event.

Creating a pool (pull_created - measurement):
```
project - tag
repo -tag
author -tag
pull_id -tag (gitlab iid or stash pullid)
jira - tag (jira key)
created - field (boolean)
```

Approve (pull_approved - measurement):
```
project - tag
repo -tag
author -tag
approver - tag
pull_id -tag (gitlab iid or stash pullid)
jira - tag (jira key)
approved- tag
since_created_sec - field
```

Withdrawal of approval (pull_approved - measurement):
```
project - tag
repo -tag
author -tag
approver - tag
pull_id -tag (gitlab iid or stash pullid)
jira - tag (jira key)
approved- tag
since_created_sec - field
```

Close (pull_closed - measurement):
```
project - tag
repo -tag
author -tag
closer - tag
pull_id -tag (gitlab iid or stash pullid)
jira - tag (jira key)
since_created_sec - field
```

Merge (pull_merged - measurement):
```
project - tag
repo -tag
author -tag
merger - tag
pull_id -tag (gitlab iid or stash pullid)
jira - tag (jira key)
since_created_sec - field
```
## Custom emoji
Bot uses a few custom emojis. Pictures are in the emoji folder.
Emoji mappings:
```
gitlab.png - :gitlab:
jira.png - :jira:
new_stash.png - :new_stash:
pullrequest.png - :pullrequest:
use_threads.png - :use_threads:
``` 
## License
Apache 2.0
