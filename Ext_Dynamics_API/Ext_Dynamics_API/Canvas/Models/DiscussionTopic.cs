using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas.Models
{
    public class DiscussionTopic
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty("posted_at")]
        public DateTime PostedAt { get; set; }

        [JsonProperty("last_reply_at")]
        public DateTime? LastReplyAt { get; set; }

        [JsonProperty("require_initial_post")]
        public bool RequireInitialPost { get; set; }

        [JsonProperty("user_can_see_posts")]
        public bool UserCanSeePosts { get; set; }

        [JsonProperty("discussion_subentry_count")]
        public int DiscussionSubentryCount { get; set; }

        [JsonProperty("read_state")]
        public string ReadState { get; set; }

        [JsonProperty("unread_count")]
        public int UnreadCount { get; set; }

        [JsonProperty("subscribed")]
        public bool Subscribed { get; set; }

        [JsonProperty("subscription_hold")]
        public string SubscriptionHold { get; set; }

        [JsonProperty("assignment_id")]
        public int? AssignmentId { get; set; }

        [JsonProperty("delayed_post_at")]
        public DateTime? DelayedPostAt { get; set; }

        [JsonProperty("published")]
        public bool Published { get; set; }

        [JsonProperty("lock_at")]
        public DateTime? LockAt { get; set; }

        [JsonProperty("locked")]
        public bool Locked { get; set; }

        [JsonProperty("pinned")]
        public bool Pinned { get; set; }

        [JsonProperty("locked_for_user")]
        public bool LockedForUser { get; set; }

        [JsonProperty("lock_info")]
        public LockInfo LockInfo { get; set; }

        [JsonProperty("lock_explanation")]
        public string LockExplanation { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("topic_children")]
        public List<int> TopicChildren { get; set; }

        [JsonProperty("group_topic_children")]
        public List<GroupTopicChildren> GroupTopicChildren { get; set; }

        [JsonProperty("root_topic_id")]
        public int? RootTopicId { get; set; }

        [JsonProperty("podcast_url")]
        public string PodcastUrl { get; set; }

        [JsonProperty("discussion_type")]
        public string DiscussionType { get; set; }

        [JsonProperty("group_category_id")]
        public int? GroupCategoryId { get; set; }

        [JsonProperty("attachments")]
        public List<FileAttachment> Attachments { get; set; }

        [JsonProperty("permissions")]
        public Permissions Permissions { get; set; }

        [JsonProperty("allow_rating")]
        public bool AllowRating { get; set; }

        [JsonProperty("only_graders_can_rate")]
        public bool OnlyGradersCanRate { get; set; }

        [JsonProperty("sort_by_rating")]
        public bool SortByRating { get; set; }
    }
}
