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
        }
        public enum ValueType { OldValue, NewValue }
        public static object GetValue(Change[] changes, string key, ValueType type)
        {
            foreach (var value in changes)
            {
                if (value.Key == key)
                {
                    if (type == ValueType.OldValue)
                        return value.OldValue;
                    else
                        return value.NewValue;
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
                        Text = "**<user>** created the channel **<channel>**".Replace("<channel>", GetValue(changes, "name", ValueType.OldValue).ToString());
                        break;
                    }
                    case AuditLogActionType.ChannelUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = "<user> updated the channel <target>";
                        if (LocalState.Guilds[guildId].channels.ContainsKey(targetid))
                            Text = "**<user>** updated the channel **<channel>**".Replace("<channel>", LocalState.Guilds[guildId].channels[targetid].raw.Name);
                        else
                            Text = "**<user>** updated a now deleted channel (**<channelid>**)".TryReplace("<channelid>", targetid);
                        break;
                    }
                    case AuditLogActionType.ChannelDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = "**<user>** deleted the channel **<channel>**".TryReplace("<channel>", GetValue(changes, "name", ValueType.OldValue).ToString());
                        break;
                    }
                    case AuditLogActionType.ChannelOverwriteCreate:
                    {
                        Glyph = "";
                        Color = "online";
                        break;
                    }
                    case AuditLogActionType.ChannelOverwriteUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        break;
                        }
                    case AuditLogActionType.ChannelOverwriteDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        break;
                    }
                    case AuditLogActionType.EmojiCreate:
                    {
                        Glyph = "";
                        Color = "online";
                        Text = "**<user>** created the emoji **<emojiname>**".TryReplace("<emojiname>", GetValue(changes, "name", ValueType.NewValue).ToString());
                        break;
                    }
                    case AuditLogActionType.EmojiUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = "**<user>** updated the emoji **<emojiname>**".TryReplace("<emojiname>", GetValue(changes, "name", ValueType.NewValue).ToString());
                        break;
                    }
                    case AuditLogActionType.EmojiDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = "**<user>** deleted the emoji **<emojiname>**".TryReplace("<emojiname>", GetValue(changes, "name", ValueType.OldValue).ToString());
                        break;
                    }
                    case AuditLogActionType.GuildUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = "**<user>** updated the server info";
                        break;
                    }
                    case AuditLogActionType.InviteCreate:
                    {
                        Glyph = "";
                        Color = "online";
                        Text = "<user> created the invite <code>".TryReplace("<code>", GetValue(changes, "code", ValueType.NewValue).ToString());
                        break;
                    }
                    case AuditLogActionType.InviteUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = "**<user>** updated the invite **<code>**".TryReplace("<code>", GetValue(changes, "code", ValueType.NewValue).ToString());
                        break;
                    }
                    case AuditLogActionType.InviteDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = "**<user>** deleted an invite";
                        break;
                    }
                    case AuditLogActionType.MemberBanAdd:
                    {
                        Glyph = "";
                        Color = "dnd";
                        break;
                    }
                    case AuditLogActionType.MemberBanRemove:
                    {
                        Glyph = "";
                        Color = "online";
                        Text = "**<user>** banned **<banneduser>**".TryReplace("<banneduser>", users[targetid].Username);
                        break;
                    }
                    case AuditLogActionType.MemberKick:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = "**<user>** kicked **<kickeduser>**".TryReplace("<kickeduser>", users[targetid].Username);
                        break;
                    }
                    case AuditLogActionType.MemberPrune:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = "**<user>** pruned the server";
                        break;
                    }
                    case AuditLogActionType.MemberRoleUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = "**<user>** updated the role of **<user2>**".TryReplace("<user2>", users[targetid].Username);
                        break;
                    }
                    case AuditLogActionType.MemberUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        Text = "**<user>** updated **<user2>**".TryReplace("<user2>", users[targetid].Username);
                        break;
                    }
                    case AuditLogActionType.MessageDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        var channelname = "<deleted channel>";
                        if (options.ChannelId != null)
                        {
                            if (LocalState.Guilds[guildId].channels.ContainsKey(options.ChannelId))
                                channelname = "#" + LocalState.Guilds[guildId].channels[options.ChannelId].raw.Name;
                        }
                        
                        Text = "**<user>** deleted a message by **<user2>** in **<channel>**".TryReplace("<user2>", users[targetid].Username).Replace("<channel>", channelname);
                        break;
                        }
                    case AuditLogActionType.RoleCreate:
                    {
                        Glyph = "";
                        Color = "online";

                        Text = "**<user>** created the role **<role>**".TryReplace("<role>", GetValue(changes, "name", ValueType.NewValue).ToString());
                            break;
                    }
                    case AuditLogActionType.RoleUpdate:
                    {
                        Glyph = "";
                        Color = "idle";
                        string rolename = "";
                        if (LocalState.Guilds[guildId].roles.ContainsKey(targetid))
                            rolename = LocalState.Guilds[guildId].roles[targetid].Name;
                        else
                        {
                            object val = GetValue(changes, "name", ValueType.NewValue);
                            if (val != null) rolename = val.ToString();
                            else rolename = "<deleted role>";
                        }
                         Text = "**<user>** modified the role **<role>**".TryReplace("<role>", rolename);
                            break;
                    }
                    case AuditLogActionType.RoleDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = "**<user>** deleted the role **<role>**".TryReplace("<role>", GetValue(changes, "name", ValueType.OldValue).ToString());
                            break;
                    }
                    case AuditLogActionType.WebhookCreate:
                    {
                        Glyph = "";
                        Color = "online";
                        Text = "**<user>** created the webhook **<webhook>**".TryReplace("<webhook>", GetValue(changes, "name", ValueType.NewValue).ToString());
                            break;
                    }
                    case AuditLogActionType.WebhookDelete:
                    {
                        Glyph = "";
                        Color = "dnd";
                        Text = "**<user>** deleted the webhook **<webhook>**".TryReplace("<webhook>", GetValue(changes, "name", ValueType.OldValue).ToString());
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
                        Text = "**<user>** updated the webhook **<webhook>**".TryReplace("<webhook>", webhookname);
                            break;
                    }
                    default:
                    {
                        Glyph = "";
                        Text = "**<user>** performed an unknown action";
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
                    if (auditlog.Users != null)
                    {
                        foreach (var user in auditlog.Users)
                        {
                            users.Add(user.Id, user);
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
                            if(action.Text != null)
                                se.Text = action.Text.Replace("<user>", users[entry.UserId].Username);
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
