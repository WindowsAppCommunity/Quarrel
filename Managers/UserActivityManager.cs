using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.UserActivities;
using Windows.UI.Core;
using Windows.UI.Shell;

namespace Discord_UWP.Managers
{
    class UserActivityManager
    {
        static string timelinecard = "{\"$schema\":\"http://adaptivecards.io/schemas/adaptive-card.json\",\"type\":\"AdaptiveCard\",\"version\":\"1.0\",\"body\":[{\"type\":\"Container\",\"items\":[{\"type\":\"ColumnSet\",\"columns\":[{\"type\":\"Column\",\"width\":\"auto\",\"items\":[{\"type\":\"Image\",\"url\":\"$IMAGE\",\"size\":\"medium\",\"style\":\"person\"}]},{\"type\":\"Column\",\"width\":\"stretch\",\"items\":[{\"type\":\"TextBlock\",\"text\":\"$TITLE\",\"weight\":\"bolder\",\"size\":\"extralarge\",\"wrap\":true},{\"type\":\"TextBlock\",\"spacing\":\"none\",\"text\":\"$SUBTITLE\",\"size\":\"medium\",\"isSubtle\":true,\"wrap\":true}]}]}]}]}";
        static UserActivitySession _currentActivity;
        static UserActivity userActivity;
        static UserActivityChannel channel;

        //Every time the user switches to another DM or to another guild, the current activity is disposed and a new one is created
        public static void SwitchSession(string NewGuildId)
        {
            if (_currentActivity != null)
            {
                //Dispose of any current UserActivitySession
                _currentActivity?.Dispose();
            }
            //userActivity = await channel.GetOrCreateUserActivityAsync(NewGuildId);
           
        }

        //Every time the user sends a message
        public static async Task GenerateActivityAsync(string GuildId, string GuildName, string GuildImage, string ChannelId, string ChannelName)
        {
            channel = UserActivityChannel.GetDefault();
            if (GuildId == "DMs")
                userActivity = await channel.GetOrCreateUserActivityAsync(ChannelId);
            else
                userActivity = await channel.GetOrCreateUserActivityAsync(GuildId);

            //Populate required properties
                timelinecard = timelinecard.Replace("$TITLE", GuildName);
                timelinecard = timelinecard.Replace("$SUBTITLE", ChannelName);
                timelinecard = timelinecard.Replace("$IMAGE", GuildImage);

              userActivity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(timelinecard);
            //userActivity.VisualElements.DisplayText = "Hello Activities";

            userActivity.ActivationUri = new Uri("discorduwp://Navigate/Guild/"+ GuildId+ "/"+ChannelId);
            userActivity.ContentUri = new Uri(GuildImage);
            userActivity.ContentInfo = UserActivityContentInfo.FromJson("{\"@context\":\"~~http~~://schema.org\",\"@type\": \"CommunicateAction\",\"subjectOf\": \""+ChannelName+"\"}");
            userActivity.FallbackUri = new Uri("http://discordapp.com/channels/" + GuildId + "/" + ChannelId);

            
            //Save
            await userActivity.SaveAsync(); //save the new metadata

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                _currentActivity = userActivity.CreateSession();
            });
            

        }
    }
}
