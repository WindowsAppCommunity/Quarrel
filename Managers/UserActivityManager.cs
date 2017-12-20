using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;

namespace Discord_UWP.Managers
{
    class UserActivityManager
    {
        UserActivitySession _currentActivity;

        private async Task GenerateActivityAsync(string GuildId, string GuildName, string ChannelId, string ChannelName)
        {
            //Get the default UserActivityChannel and query it for our UserActivity. If the activity doesn't exist, one is created.
            UserActivityChannel channel = UserActivityChannel.GetDefault();
            UserActivity userActivity = await channel.GetOrCreateUserActivityAsync("MainPage");

            //Populate required properties
            userActivity.VisualElements.DisplayText = "Hello Activities";
            userActivity.ActivationUri = new Uri("discorduwp://Navigate/Guild/"+ GuildId+ "/"+ChannelId);

            //Save
            await userActivity.SaveAsync(); //save the new metadata

            //Dispose of any current UserActivitySession, and create a new one.
            _currentActivity?.Dispose();
            _currentActivity = userActivity.CreateSession();
        }
    }
}
