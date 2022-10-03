﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SlackPrBot.DomainModels;

namespace SlackPrBot.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20200402120653_ClosedStatus")]
    partial class ClosedStatus
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0");

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.Comment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT");

                    b.Property<int>("CommentId")
                        .HasColumnName("comment_id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnName("text")
                        .HasColumnType("TEXT");

                    b.Property<string>("author_id")
                        .HasColumnType("TEXT");

                    b.Property<string>("pull_request_id")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("author_id");

                    b.HasIndex("pull_request_id");

                    b.ToTable("comment");
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.CommentSlack", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT");

                    b.Property<string>("SlackTs")
                        .IsRequired()
                        .HasColumnName("slack_ts")
                        .HasColumnType("TEXT");

                    b.Property<string>("comment_id")
                        .HasColumnType("TEXT");

                    b.Property<string>("pull_request_slack_id")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("comment_id");

                    b.HasIndex("pull_request_slack_id");

                    b.ToTable("comment_slack");
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.PullRequest", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("TEXT");

                    b.Property<string>("FromRef")
                        .IsRequired()
                        .HasColumnName("from_ref")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Iid")
                        .HasColumnName("iid")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastActiveDate")
                        .HasColumnName("last_active_date")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasColumnName("project_name")
                        .HasColumnType("TEXT");

                    b.Property<int>("PullId")
                        .HasColumnName("pull_id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RepoName")
                        .IsRequired()
                        .HasColumnName("repo_name")
                        .HasColumnType("TEXT");

                    b.Property<string>("TaskId")
                        .IsRequired()
                        .HasColumnName("task_id")
                        .HasColumnType("TEXT");

                    b.Property<string>("author_id")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FromRef");

                    b.HasIndex("author_id");

                    b.ToTable("pull_request");
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.PullRequestChannel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastNotificationDate")
                        .HasColumnName("last_notification_date")
                        .HasColumnType("TEXT");

                    b.Property<string>("SlackTs")
                        .IsRequired()
                        .HasColumnName("slack_ts")
                        .HasColumnType("TEXT");

                    b.Property<string>("pull_request_id")
                        .HasColumnType("TEXT");

                    b.Property<string>("settings_id")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("pull_request_id");

                    b.HasIndex("settings_id");

                    b.ToTable("pull_request_slack");
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.PullRequestWatch", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT");

                    b.Property<string>("pull_request_id")
                        .HasColumnType("TEXT");

                    b.Property<string>("user_id")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("pull_request_id");

                    b.HasIndex("user_id");

                    b.ToTable("pull_request_watch");
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.Reviewer", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnName("status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("pull_request_id")
                        .HasColumnType("TEXT");

                    b.Property<string>("user_id")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("pull_request_id");

                    b.HasIndex("user_id");

                    b.ToTable("reviewer");
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.Settings", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasColumnName("channel_id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClosedStatuses")
                        .HasColumnName("closed_statuses")
                        .HasColumnType("TEXT");

                    b.Property<string>("DevelopmentStatuses")
                        .HasColumnName("development_statuses")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsGitlab")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("is_gitlab")
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false);

                    b.Property<bool>("JiraSupport")
                        .HasColumnName("jira_support")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ReadyStatuses")
                        .HasColumnName("ready_statuses")
                        .HasColumnType("TEXT");

                    b.Property<string>("ReadyToReviewStatuses")
                        .HasColumnName("ready_to_review_statuses")
                        .HasColumnType("TEXT");

                    b.Property<string>("ReviewStatuses")
                        .HasColumnName("review_statuses")
                        .HasColumnType("TEXT");

                    b.Property<string>("StashProject")
                        .IsRequired()
                        .HasColumnName("stash_project")
                        .HasColumnType("TEXT");

                    b.Property<string>("StashRepo")
                        .IsRequired()
                        .HasColumnName("stash_repo")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("StashStaleTime")
                        .HasColumnName("stash_stale_time")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId", "StashProject", "StashRepo")
                        .IsUnique();

                    b.ToTable("settings");
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnName("email")
                        .HasColumnType("TEXT");

                    b.Property<string>("SlackUserId")
                        .IsRequired()
                        .HasColumnName("slack_user_id")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("user");
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.Comment", b =>
                {
                    b.HasOne("SlackPrBot.DomainModels.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("author_id");

                    b.HasOne("SlackPrBot.DomainModels.Entities.PullRequest", "PullRequest")
                        .WithMany("Comments")
                        .HasForeignKey("pull_request_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.CommentSlack", b =>
                {
                    b.HasOne("SlackPrBot.DomainModels.Entities.Comment", "Comment")
                        .WithMany("SlackMessages")
                        .HasForeignKey("comment_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SlackPrBot.DomainModels.Entities.PullRequestChannel", "PullRequestChannel")
                        .WithMany()
                        .HasForeignKey("pull_request_slack_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.PullRequest", b =>
                {
                    b.HasOne("SlackPrBot.DomainModels.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("author_id");
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.PullRequestChannel", b =>
                {
                    b.HasOne("SlackPrBot.DomainModels.Entities.PullRequest", "PullRequest")
                        .WithMany("ChannelMap")
                        .HasForeignKey("pull_request_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SlackPrBot.DomainModels.Entities.Settings", "Settings")
                        .WithMany()
                        .HasForeignKey("settings_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.PullRequestWatch", b =>
                {
                    b.HasOne("SlackPrBot.DomainModels.Entities.PullRequest", "PullRequest")
                        .WithMany("Watch")
                        .HasForeignKey("pull_request_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SlackPrBot.DomainModels.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("user_id");
                });

            modelBuilder.Entity("SlackPrBot.DomainModels.Entities.Reviewer", b =>
                {
                    b.HasOne("SlackPrBot.DomainModels.Entities.PullRequest", "PullRequest")
                        .WithMany("Reviewers")
                        .HasForeignKey("pull_request_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SlackPrBot.DomainModels.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
