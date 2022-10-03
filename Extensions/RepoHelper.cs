using System;

namespace SlackPrBot.Extensions
{
    public static class RepoHelper
    {

        public static string PullRequestPath(bool isGitlab, string baseRepoPath, string project, string repo, int pullId, int? iid, bool bold = false)
        {
            if (!isGitlab)
            {
                return $"{baseRepoPath}/projects/{project}/repos/{repo}/pull-requests/{pullId}/overview|:new_stash: {(bold ? "*" : "")}{pullId}{(bold ? "*" : "")}";
            }

            return $"{baseRepoPath}/{project}/{repo}/-/merge_requests/{iid}|:gitlab: {(bold ? "*" : "")}{iid}{(bold ? "*" : "")}";
        }

        public static string PullRequestCommentPath(bool isGitlab, string baseRepoPath, string project, string repo, int pullId, int? iid, int commentId)
        {
            if (!isGitlab)
            {
                return $"{baseRepoPath}/projects/{project}/repos/{repo}/pull-requests/{pullId}/overview?commentId={commentId}|:new_stash: comment";
            }

            return $"{baseRepoPath}/{project}/{repo}/-/merge_requests/{iid}#note_{commentId}|:gitlab: comment";
        }

        public static string PullRequestCommitPath(bool isGitlab, string baseRepoPath, string project, string repo, int? iid, string commitId)
        {
            if (!isGitlab)
            {
                return $"{baseRepoPath}/projects/{project}/repos/{repo}/commits/{commitId}|:new_stash: commit";
            }

            return $"{baseRepoPath}/{project}/{repo}/-/merge_requests/{iid}/diffs?commit_id={commitId}|:gitlab: commit";
        }

        public static string RepoPath(bool isGitlab, string baseRepoPath, string project, string repo)
        {
            if (!isGitlab)
            {
                return $":new_stash: {baseRepoPath}/projects/{project}/repos/{repo}";
            }

            return $":gitlab: {baseRepoPath}/{project}/{repo}";
        }
    }
}
