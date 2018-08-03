using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;
using DiscordAPI.API.Guild.Models;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    public static class Extensions
    {

    public static string TryReplace(this String str, string source, string target)
    {
        if (target == null) return str.Replace(source, "");
        else return str.Replace(source, target);
    }
        public static string Bold(this String str)
        {
            return ("**" + str + "**");
        }


        public static string ReplaceChannel(this String str, string guildId, string targetid)
        {
            if (!LocalState.Guilds[guildId].channels.ContainsKey(targetid)) return "<deleted channel>";
            var channel = LocalState.Guilds[guildId].channels[targetid];
            if (channel.raw.Type == 0)
            {
                return str.Replace("<channel>", "<#" + channel.raw.Id + ">");
            }
            else
            {
                return "**" + channel.raw.Name + "**";
            }
        }
        public static string ReplaceRole(this String str, string guildId, string targetid)
        {
            if (!LocalState.Guilds[guildId].roles.ContainsKey(targetid)) return "<deleted role>";
            return str.Replace("<role>", "<@&" + targetid + ">");
        }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    ///
    public sealed partial class AuditLog : Page
    {
        public static string GetByNumber(String str, Int64 number)
        {
            switch (number)
            {
                case 0: return App.GetString(str + "Zero");
                case 1: return App.GetString(str + "One");
                case 2: return App.GetString(str + "Two");
                case 3: return App.GetString(str + "Three");
                case 4: return App.GetString(str + "Four");
                case 5: return App.GetString(str + "Five");
                case 6: return App.GetString(str + "Six");
                case 7: return App.GetString(str + "Seven");
                case 8: return App.GetString(str + "Eight");
                case 9: return App.GetString(str + "Nine");
                default: return "<unknown>";
            }
        }
        public class SimpleAuditLogEntry
        {
            public string Glyph { get; set; }
            public SolidColorBrush Brush { get; set; }
            public string Avatar { get; set; }
            public string Time { get; set; }
            public string Text { get; set; }
            public List<User> Users { get; set; }
            public List<string> Subactions { get; set; }
            public AuditLogActionType Action { get; set; }
        }
        public enum ValueType { OldValue, NewValue }
        public static object GetValue(Change[] changes, string key, ValueType type)
        {
            foreach (var value in changes)
            {
                if (value.Key == key)
                {
                    if (type == ValueType.OldValue)
                    {
                        return value.OldValue;
                    }

                    else
                    {
                        return value.NewValue;
                    }

                }
            }
            return null;


        }

        public static void SubactionFromPermissions(Change[] changes, ref List<string> SubAction, Options options, Dictionary<string, AuditLogUser> users, string guildId)
        {
            foreach (var change in changes)
            {
                if (change.NewValue is int PermInt && PermInt != 0 && (change.Key == "deny" || change.Key == "allow"))
                {
                    string permission = "`" + new Permissions(PermInt).GetFirstPermission() + "`";
                    string role = "";
                    if (options.Type == "member" && options.Id != null) role = "<@" + options.Id + ">";
                    else if(options.Type == "role")
                    {
                        if (options.Id != null && LocalState.Guilds[guildId].roles.ContainsKey(options.Id))
                            role = "<@&" + options.Id + ">";
                        else if (options.RoleName != null)
                            role = options.RoleName.Bold();
                    }
                    if(change.Key == "allow")
                    SubAction.Add(App.GetString("/Dialogs/AuditLogPermissionGiven").TryReplace("<permission>", permission).TryReplace("<role>", role));
                    else
                    SubAction.Add(App.GetString("/Dialogs/AuditLogPermissionRevoked").TryReplace("<permission>", permission).TryReplace("<role>", role));
                }
            }
        }
        /// <summary>
        /// Add the strings for changes from a list of SubActions
        /// </summary>
        /// <param name="changes">The audit log changes</param>
        /// <param name="SubAction">The SubAction list</param>
        public static void SubactionFromChanges(Change[] changes, ref List<string> SubAction)
        {
            foreach (var change in changes)
            {
                switch (change.Key)
                {
                    case "bitrate":
                        if (change.OldValue == null) SubAction.Add(App.GetString("/Dialogs/AuditLogChannelUpdateBitrate").TryReplace("<bitrate>", (change.NewValue+"Kbps").Replace("000", "").Bold()));
                        else SubAction.Add(App.GetString("/Dialogs/AuditLogChannelUpdateBitrateChange").TryReplace("<new>", (change.NewValue + "Kbps").Replace("000", "").Bold()).TryReplace("<old>", (change.OldValue + "Kbps").Replace("000", "").Bold()));
                        break;
                    case "topic":
                        if(change.OldValue == null) SubAction.Add(App.GetString("/Dialogs/AuditLogChannelUpdateTopic").TryReplace("<topic>", change.NewValue.ToString().Bold()));
                        else SubAction.Add(App.GetString("/Dialogs/AuditLogChannelUpdateTopicChange").TryReplace("<new>", change.NewValue.ToString().Bold()).TryReplace("<old>", change.OldValue.ToString().Bold()));
                        break;
                    case "name":
                        SubAction.Add(App.GetString("/Dialogs/AuditLogChannelUpdateName").TryReplace("<name>", change.NewValue.ToString().Bold()));
                        break;
                    case "nsfw":
                        if (((bool)change.NewValue)) SubAction.Add(App.GetString("/Dialogs/AuditLogChannelUpdateNsfwTrue"));
                        else SubAction.Add(App.GetString("/Dialogs/AuditLogChannelUpdateNsfwFalse"));
                        break;
                    case "afk_timeout":
                        SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateAfkTimeout").TryReplace("<time>", (((Int64)change.NewValue)/60).ToString().Bold()));
                        break;
                    case "afk_channel":
                        SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateAfkChannel").TryReplace("<channel>", "<#"+change.NewValue.ToString()+">"));
                        break;
                    case "region":
                        if (change.OldValue == null) SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateRegion").TryReplace("<region>", change.NewValue.ToString().Bold()));
                        else SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateRegionChange").TryReplace("<new>", change.NewValue.ToString().Bold()).TryReplace("<old>", change.OldValue.ToString().Bold()));
                        break;
                    case "icon_hash":
                        SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateIcon"));
                        break;
                    case "splash_hash":
                        SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateSplash"));
                        break;
                    case "mfa_level":
                        if ((Int64)change.NewValue == 1) SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateMfaEnabled"));
                        else SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateMfaDisabled"));
                        break;
                    case "explicit_content_filter":
                        if (change.OldValue == null)
                            SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateExplicitFilter")
                                .TryReplace("<level>", GetByNumber("/Dialogs/AuditLogServerUpdateExplicitFilter", (Int64)change.NewValue).Bold()));
                        else
                            SubAction.Add(App.GetString("/Dialogs/AuditLogServerExplicitFilterChange")
                                .TryReplace("<new>", GetByNumber("/Dialogs/AuditLogServerUpdateExplicitFilter", (Int64)change.NewValue).Bold())
                                .TryReplace("<old>", GetByNumber("/Dialogs/AuditLogServerUpdateExplicitFilter", (Int64)change.OldValue).Bold()));
                        break;
                    case "verification_level":
                        if(change.OldValue == null)
                            SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateVerification")
                                .TryReplace("<level>", App.GetString("/Dialogs/AuditLogServerUpdateVerification"+change.NewValue.ToString().Bold())));
                        else
                            SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateVerificationChange")
                                .TryReplace("<new>", GetByNumber("/Dialogs/AuditLogServerUpdateVerification", (Int64)change.NewValue).Bold())
                                .TryReplace("<old>", GetByNumber("/Dialogs/AuditLogServerUpdateVerification", (Int64)change.OldValue).Bold()));
                        break;
                    case "default_message_notifications":
                        if ((Int64)change.NewValue == 1) SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateNotificationOne"));
                        else SubAction.Add(App.GetString("/Dialogs/AuditLogServerUpdateNotificationZero"));
                        break;
                }
            }
        }
        public class AuditLogAction
        {
            public AuditLogAction(AuditLogActionType type, string guildId, string targetid, Dictionary<string, AuditLogUser> users, Dictionary<string, Webhook> webhooks, Change[] changes, Options options)
            {
                switch (type)
                {
                    case AuditLogActionType.ChannelCreate:
                    {
                        Glyph = "";
                        Color = "online";
                        Text = App.GetString("/Dialogs/AuditLogChannelCreate").ReplaceChannel(guildId, targetid);
                        SubactionFromChanges(changes, ref SubAction);
                        break;
                    }
                    case AuditLogActionType.ChannelUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        if (LocalState.Guilds[guildId].channels.ContainsKey(targetid))
                            Text = App.GetString("/Dialogs/AuditLogChannelUpdateValid").ReplaceChannel(guildId, targetid);
                        else
                            Text = App.GetString("/Dialogs/AuditLogChannelUpdateInvalid").TryReplace("<channelid>", targetid);
                            SubactionFromChanges(changes, ref SubAction);
                            break;
                    }
                    case AuditLogActionType.ChannelDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = App.GetString("/Dialogs/AuditLogChannelDelete").TryReplace("<channel>", GetValue(changes, "name", ValueType.OldValue)?.ToString().Bold());
                        break;
                    }
                    case AuditLogActionType.ChannelOverwriteCreate:
                    {
                        Glyph = "";
                        Color = "online";
                        if (LocalState.Guilds[guildId].channels.ContainsKey(targetid))
                            Text = App.GetString("/Dialogs/AuditLogChannelPermissionCreateValid").ReplaceChannel(guildId, targetid);
                        else
                            Text = App.GetString("/Dialogs/AuditLogChannelPermissionCreateInvalid");
                        SubactionFromPermissions(changes, ref SubAction, options, users, guildId);
                        break;
                    }
                    case AuditLogActionType.ChannelOverwriteUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        if (LocalState.Guilds[guildId].channels.ContainsKey(targetid))
                            Text = App.GetString("/Dialogs/AuditLogChannelPermissionUpdateValid").ReplaceChannel(guildId, targetid);
                        else
                            Text = App.GetString("/Dialogs/AuditLogChannelPermissionUpdateInvalid");
                        SubactionFromPermissions(changes, ref SubAction, options, users, guildId);
                            break;
                        }
                    case AuditLogActionType.ChannelOverwriteDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        if (LocalState.Guilds[guildId].channels.ContainsKey(targetid))
                            Text = App.GetString("/Dialogs/AuditLogChannelPermissionRemoveValid").ReplaceChannel(guildId, targetid);
                            else
                            Text = App.GetString("/Dialogs/AuditLogChannelPermissionRemoveInvalid");
                        break;
                    }
                    case AuditLogActionType.EmojiCreate:
                    {
                        Glyph = "";
                        Color = "online";
                        Text = App.GetString("/Dialogs/AuditLogEmojiCreate").TryReplace("<emojiname>", GetValue(changes, "name", ValueType.NewValue)?.ToString());
                        break;
                    }
                    case AuditLogActionType.EmojiUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = App.GetString("/Dialogs/AuditLogEmojiUpdate").TryReplace("<emojiname>", GetValue(changes, "name", ValueType.NewValue)?.ToString());
                        break;
                    }
                    case AuditLogActionType.EmojiDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = App.GetString("/Dialogs/AuditLogEmojiDelete").TryReplace("<emojiname>", GetValue(changes, "name", ValueType.OldValue)?.ToString());
                        break;
                    }
                    case AuditLogActionType.GuildUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = App.GetString("/Dialogs/AuditLogGuildUpdate");
                        SubactionFromChanges(changes, ref SubAction);
                            break;
                    }
                    case AuditLogActionType.InviteCreate:
                    {
                        Glyph = "";
                        Color = "online";
                        Text = App.GetString("/Dialogs/AuditLogInviteCreate").TryReplace("<code>", GetValue(changes, "code", ValueType.NewValue)?.ToString().Bold());
                        break;
                    }
                    case AuditLogActionType.InviteUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = App.GetString("/Dialogs/AuditLogInviteUpdate").TryReplace("<code>", GetValue(changes, "code", ValueType.NewValue)?.ToString().Bold());
                        break;
                    }
                    case AuditLogActionType.InviteDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = App.GetString("/Dialogs/AuditLogInviteDelete").TryReplace("<code>", GetValue(changes, "code", ValueType.OldValue)?.ToString().Bold());
                        break;
                    }
                    case AuditLogActionType.MemberBanAdd:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = App.GetString("/Dialogs/AuditLogMemberBanAdd").TryReplace("<banneduser>", "<@"+targetid+">");
                        break;
                    }
                    case AuditLogActionType.MemberBanRemove:
                    {
                        Glyph = "";
                        Color = "online";
                        Text = App.GetString("/Dialogs/AuditLogMemberBanRemove").TryReplace("<banneduser>", "<@"+targetid+">");
                        break;
                    }
                    case AuditLogActionType.MemberKick:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = App.GetString("/Dialogs/AuditLogMemberKick").TryReplace("<kickeduser>", "<@" + targetid + ">");
                        break;
                    }
                    case AuditLogActionType.MemberPrune:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = App.GetString("/Dialogs/AuditLogMemberPrune");
                        break;
                    }
                    case AuditLogActionType.MemberRoleUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = App.GetString("/Dialogs/AuditLogMemberRoleUpdate").TryReplace("<user2>", "<@" + targetid + ">");
                        break;
                    }
                    case AuditLogActionType.MemberUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = App.GetString("/Dialogs/AuditLogMemberUpdate").TryReplace("<user2>", "<@" + targetid + ">");
                        break;
                    }
                    case AuditLogActionType.MessageDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        if(!options.Count.HasValue || options.Count == 1)
                            Text = App.GetString("/Dialogs/AuditLogMessageDelete").TryReplace("<user2>", "<@" + targetid + ">").ReplaceChannel(guildId, options.ChannelId);
                        else
                            Text = App.GetString("/Dialogs/AuditLogMessageDeletePlural").TryReplace("<user2>", "<@" + targetid + ">").ReplaceChannel(guildId, options.ChannelId).TryReplace("<x>", options.Count.ToString().Bold());
                        break;
                    }
                    case AuditLogActionType.RoleCreate:
                    {
                        Glyph = "";
                        Color = "online";
                        string rolename = "";
                        if (LocalState.Guilds[guildId].roles.ContainsKey(targetid))
                            rolename = "<@&" + targetid + ">";
                        else
                        {
                            object val = GetValue(changes, "name", ValueType.NewValue);
                            if (val != null) rolename = val.ToString().Bold();
                            else rolename = "<deleted role>";
                        }
                        Text = App.GetString("/Dialogs/AuditLogRoleCreate").TryReplace("<role>", rolename);
                        break;
                    }
                    case AuditLogActionType.RoleUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        string rolename = "";
                        if (LocalState.Guilds[guildId].roles.ContainsKey(targetid))
                            rolename = "<@&" + targetid + ">";
                        else
                        {
                            object val = GetValue(changes, "name", ValueType.NewValue);
                            if (val != null) rolename = val.ToString().Bold();
                            else rolename = "<deleted role>";
                        }
                         Text = App.GetString("/Dialogs/AuditLogRoleUpdate").TryReplace("<role>", rolename);
                            break;
                    }
                    case AuditLogActionType.RoleDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = App.GetString("/Dialogs/AuditLogRoleDelete").TryReplace("<role>", GetValue(changes, "name", ValueType.OldValue)?.ToString().Bold());
                            break;
                    }
                    case AuditLogActionType.WebhookCreate:
                    {
                        Glyph = "";
                        Color = "online";
                        Text = App.GetString("/Dialogs/AuditLogWebhookCreate").TryReplace("<webhook>", GetValue(changes, "name", ValueType.NewValue)?.ToString().Bold());
                            break;
                        }
                    case AuditLogActionType.WebhookUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        var webhookname = "";
                        foreach (var webhook in webhooks)
                        {
                            if (webhook.Key == targetid)
                            {
                                webhookname = webhook.Value.Name;
                            }
                        }
                        Text = App.GetString("/Dialogs/AuditLogWebhookUpdate").TryReplace("<webhook>", webhookname.Bold());
                        break;
                    }
                    case AuditLogActionType.WebhookDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = App.GetString("/Dialogs/AuditLogWebhookDelete").TryReplace("<webhook>", GetValue(changes, "name", ValueType.OldValue)?.ToString().Bold());
                            break;
                    }
                    default:
                    {
                        Glyph = "";
                        Text = App.GetString("/Dialogs/AuditLogUnknown");
                            break;
                    }
                }
            }
            public string Glyph { get; set; }
            public string Color { get; set; }
            public string Text { get; set; }
            public List<string> SubAction = new List<string>();
        }
        public AuditLog()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                var auditlog = await RESTCalls.GetAuditLog(e.Parameter.ToString());
                LoadingRing.Visibility = Visibility.Collapsed;
                if (auditlog == null)
                {
                    //TODO Add error message
                }
                else
                {
                    Dictionary<string, AuditLogUser> users = new Dictionary<string, AuditLogUser>();
                    Dictionary<string, Webhook> webhooks = new Dictionary<string, Webhook>();
                    List<User> usersForMD = new List<User>();
                   
                    if (auditlog.Users != null)
                    {
                        foreach (var user in auditlog.Users)
                        {
                            users.Add(user.Id, user);
                            usersForMD.Add(new User(){Avatar = user.Avatar, Username = user.Username, Id = user.Id, Discriminator= user.Discriminator, });
                        }
                    }
                    if (auditlog.Webhooks != null)
                    {
                        foreach (var webhook in auditlog.Webhooks)
                        {
                            webhooks.Add(webhook.Id, webhook);
                        }
                    }
                    if (auditlog.AuditLogEntries != null)
                    {
                        List<SimpleAuditLogEntry> RawEntries = new List<SimpleAuditLogEntry>();
                        foreach (var entry in auditlog.AuditLogEntries)
                        {
                            SimpleAuditLogEntry se = new SimpleAuditLogEntry();
                            se.Action = (AuditLogActionType) entry.ActionType;
                            AuditLogAction action = new AuditLogAction(se.Action, e.Parameter.ToString(), entry.TargetId, users, webhooks, entry.Changes, entry.Options);
                            string avatar = "";
                            if (entry.UserId != null)
                            {
                                if (users.ContainsKey(entry.UserId))
                                {
                                    se.Avatar = Common.AvatarString(users[entry.UserId].Avatar,
                                        users[entry.UserId].Username);
                                }
                            }

                            se.Glyph = action.Glyph;
                            se.Brush = (SolidColorBrush) Application.Current.Resources[action.Color];
                            se.Users = usersForMD;
                            if (action.Text != null) se.Text = action.Text.Replace("<user>", "<@" + entry.UserId + ">");
                            se.Time = Common.HumanizeDate(Common.SnowflakeToTime(entry.Id).DateTime, null);
                            se.Subactions = action.SubAction;
                            RawEntries.Add(se);
                        }

                        LogItems.ItemsSource = RawEntries;
                        return;
                        List<SimpleAuditLogEntry> GroupedEntries = new List<SimpleAuditLogEntry>();
                        int subtractCount = 1;
                        for (var i = 0; i < RawEntries.Count; i++)
                        {
                            if (i > 0 && RawEntries[i].Action == RawEntries[i - 1].Action &&
                                (RawEntries[i].Action == AuditLogActionType.ChannelOverwriteUpdate ||
                                 RawEntries[i].Action == AuditLogActionType.ChannelUpdate ||
                                 RawEntries[i].Action == AuditLogActionType.EmojiUpdate ||
                                 RawEntries[i].Action == AuditLogActionType.GuildUpdate ||
                                 RawEntries[i].Action == AuditLogActionType.InviteUpdate ||
                                 RawEntries[i].Action == AuditLogActionType.MemberRoleUpdate ||
                                 RawEntries[i].Action == AuditLogActionType.RoleUpdate ||
                                 RawEntries[i].Action == AuditLogActionType.WebhookUpdate ||
                                 RawEntries[i].Action == AuditLogActionType.MessageDelete))
                            {
                                RawEntries[i-subtractCount].Subactions.AddRange(RawEntries[i].Subactions);
                                if (RawEntries[i].Action == AuditLogActionType.MessageDelete)
                                {
                                    RawEntries[i - subtractCount].Text = "Someone deleted x messages by x in #x";
                                    //Change the main message
                                }

                                if (RawEntries[i - subtractCount].Time != RawEntries[i].Time)
                                {
                                    RawEntries[i - subtractCount].Time += " - " + RawEntries[i].Time;
                                }
                            }
                        }
                        
                    }
                    
                }
            }
        }

        private void App_SubpageCloseHandler(object sender, EventArgs e)
        {
            CloseButton_Click(null, null);
            App.SubpageCloseHandler -= App_SubpageCloseHandler;
        }

        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }

    }
}
