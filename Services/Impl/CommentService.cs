using Microsoft.EntityFrameworkCore;

using SlackPrBot.DomainModels;
using SlackPrBot.DomainModels.Entities;
using SlackPrBot.Models.Slack.Api;
using SlackPrBot.Models.Stash;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace SlackPrBot.Services.Impl
{
    internal class CommentService
    {
        private readonly Context _context;
        private readonly SettingsService _settingsService;
        private readonly SlackApi _slack;
        private readonly UserService _userService;

        public CommentService(Context context, SlackApi slack, UserService userService, SettingsService settingsService)
        {
            _context = context;
            _slack = slack;
            _userService = userService;
            _settingsService = settingsService;
        }

        public async Task AddOrEditCommentAsync(CommentRequest commentRequest, bool isEdit)
        {
            var actor = commentRequest.Actor;
            var pull = commentRequest.PullRequest;
            var projectName = pull.ToRef.Repository.Project.Key;
            var repoName = pull.ToRef.Repository.Slug;
            var comment = commentRequest.Comment;
            var now = DateTime.Now;

            var settings = await _settingsService.GetSettingsByAsync(projectName, repoName);

            User parentAuthor = null;
            string parentCommentText = null;

            if (commentRequest.CommentParentId.HasValue)
            {
                var parentComment = await _context.Comments.Where(x => x.CommentId == commentRequest.CommentParentId.Value).Select(x => new { x.Author, x.Text }).FirstOrDefaultAsync();
                if (parentComment != null)
                {
                    parentAuthor = parentComment.Author;
                    parentCommentText = parentComment.Text;
                }
            }

            Comment dbComment = null;

            await _userService.AddToWatchListAsync(projectName, repoName, pull.Id, actor.EmailAddress);

            if (isEdit)
            {
                dbComment = await _context.Comments.FirstAsync(x => x.CommentId == comment.Id);
            }

            var dbPull = await _context.PullRequests.FirstOrDefaultAsync(x => x.PullId == pull.Id && x.RepoName == repoName && x.ProjectName == projectName);
            if (dbPull == null)
            {
                return;
            }

            if (dbComment != null)
            {
                var commentsSlackTs = await _context.CommentsSlack.Where(x => x.Comment.Id == dbComment.Id).Select(x => new
                {
                    x.SlackTs,
                    ParentTs = x.PullRequestChannel.SlackTs,
                    Channel = x.PullRequestChannel.Settings.ChannelId,
                    IsGitlab = x.PullRequestChannel.Settings.IsGitlab,
                    GitlabBase = x.PullRequestChannel.Settings.RepoUrl
                }).ToListAsync();
                foreach (var commentSlackTs in commentsSlackTs)
                {
                    var message = new AddOrEditComment
                    {
                        AuthorUserName = actor.Name,
                        Channel = commentSlackTs.Channel,
                        CommentId = comment.Id,
                        Message = comment.Text,
                        ProjectName = projectName,
                        RepoName = repoName,
                        PullId = pull.Id,
                        ThreadTs = commentSlackTs.ParentTs,
                        Quote = parentCommentText,
                        QuoteAuthor = parentAuthor,
                        Ts = commentSlackTs.SlackTs,
                        Iid = pull.Iid,
                        IsGitlab = commentSlackTs.IsGitlab,
                        BaseRepoPath = commentSlackTs.GitlabBase
                    };

                    await _slack.UpdateMessageAsync(message);
                }

                dbComment.Text = comment.Text;
            }
            else
            {
                var commentAuthor = await _userService.GetUserAsync(actor.EmailAddress);
                if (string.IsNullOrEmpty(commentAuthor.SlackUserId))
                {
                    return;
                }

                var commentDb = new Comment
                {
                    Text = comment.Text,
                    Author = commentAuthor,
                    CommentId = comment.Id,
                    PullRequest = dbPull,
                };

                var pullRequestsChannels = await _context.PullRequestsChannel.Where(x => x.PullRequest.Id == dbPull.Id).ToListAsync();
                foreach (var pullRequestChannel in pullRequestsChannels)
                {
                    var message = new AddOrEditComment
                    {
                        AuthorUserName = actor.Name,
                        Channel = pullRequestChannel.Settings.ChannelId,
                        CommentId = comment.Id,
                        Message = comment.Text,
                        ProjectName = projectName,
                        RepoName = repoName,
                        PullId = pull.Id,
                        ThreadTs = pullRequestChannel.SlackTs,
                        Quote = parentCommentText,
                        QuoteAuthor = parentAuthor,
                        Iid = pull.Iid,
                        IsGitlab = pullRequestChannel.Settings.IsGitlab,
                        BaseRepoPath = pullRequestChannel.Settings.RepoUrl
                    };

                    var ts = await _slack.PostMessageAsync(message);

                    commentDb.SlackMessages.Add(new CommentSlack
                    {
                        SlackTs = ts,
                        PullRequestChannel = pullRequestChannel
                    });
                }

                _context.Comments.Add(commentDb);
            }

            dbPull.LastActiveDate = now;

            await _context.SaveChangesAsync();
        }

        public async Task RemoveCommentAsync(CommentRequest commentRequest)
        {
            var actor = commentRequest.Actor;
            var pull = commentRequest.PullRequest;
            var projectName = pull.ToRef.Repository.Project.Key;
            var repoName = pull.ToRef.Repository.Slug;
            var comment = commentRequest.Comment;
            var now = DateTime.Now;

            var dbPull = await _context.PullRequests.FirstAsync(x => x.PullId == pull.Id && x.RepoName == repoName && x.ProjectName == projectName);
            var dbComment = await _context.Comments.Where(x => x.CommentId == comment.Id).FirstAsync();
            var commentsSlackTs = await _context.CommentsSlack.Where(x => x.Comment.Id == dbComment.Id).Select(x => new { x.SlackTs, ParentTs = x.PullRequestChannel.SlackTs, Channel = x.PullRequestChannel.Settings.ChannelId }).ToListAsync();

            foreach (var commentSlackTs in commentsSlackTs)
            {
                await _slack.UpdateMessageAsync(new RemoveComment
                {
                    AuthorUser = actor.Name,
                    Channel = commentSlackTs.Channel,
                    ThreadTs = commentSlackTs.ParentTs,
                    Ts = commentSlackTs.SlackTs
                });
            }

            dbPull.LastActiveDate = now;

            await _userService.AddToWatchListAsync(projectName, repoName, pull.Id, actor.EmailAddress);

            _context.Comments.Remove(dbComment);

            await _context.SaveChangesAsync();
        }
    }
}