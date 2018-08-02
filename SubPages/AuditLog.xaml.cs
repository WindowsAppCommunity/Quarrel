using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DiscordAPI.API.Guild.Models;
using Discord_UWP;
using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;
using Discord_UWP.SubPages;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    public static class Extensions
    {

    public static string TryReplace(this String str, string source, string target)
    {
        if (str == null) return str.Replace(source, "");
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

        public class SimpleAuditLogEntry
        {
            public string Glyph { get; set; }
            public SolidColorBrush Brush { get; set; }
            public string Avatar { get; set; }
            public string Time { get; set; }
            public string Text { get; set; }
            public List<User> Users { get; set; }
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
                            //Text = App.GetString("/Dialogs/AuditLogChannelCreate").Replace("<channel>", GetValue(changes, "name", ValueType.OldValue).ToString());
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
                        Text = App.GetString("/Dialogs/AuditLogMessageDelete").TryReplace("<user2>", "<@" + targetid + ">").ReplaceChannel(guildId, options.ChannelId);
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
                        foreach (var entry in auditlog.AuditLogEntries)
                        {
                            SimpleAuditLogEntry se = new SimpleAuditLogEntry();
                            AuditLogAction action = new AuditLogAction((AuditLogActionType)entry.ActionType, e.Parameter.ToString(), entry.TargetId, users, webhooks, entry.Changes, entry.Options);
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
                            se.Time = Common.HumanizeDate(DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(Common.SnowflakeToTime(entry.Id))).DateTime, null);
                            LogItems.Items.Add(se);
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
