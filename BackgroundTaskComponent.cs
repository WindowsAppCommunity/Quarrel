using Discord_UWP.API;
using Discord_UWP.API.Channel;
using Discord_UWP.API.Channel.Models;
using Discord_UWP.API.Gateway;
using Discord_UWP.API.Guild;
using Discord_UWP.API.Login;
using Discord_UWP.API.Login.Models;
using Discord_UWP.API.User;
using Discord_UWP.API.User.Models;
using Discord_UWP.Authentication;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.UI.Notifications;

namespace Discord_UWP
{
    public sealed class ToastNotificationBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral since we're executing async code
            var deferral = taskInstance.GetDeferral();
            try
            {
                // If it's a toast notification action
                if (taskInstance.TriggerDetails is ToastNotificationActionTriggerDetail)
                {
                    // Get the toast activation details
                    var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
                    // Deserialize the arguments received from the toast activation
                    QueryString args = QueryString.Parse(details.Argument);
                    // Depending on what action was taken...
                    switch (args["action"])
                    {
                        // User clicked the reply button (doing a quick reply)
                        case "reply":
                            await HandleReply(details, args);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                // Otherwise handle other background activations
                else
                    throw new NotImplementedException();
            }
            finally
            {
                // And finally release the deferral since we're done
                deferral.Complete();
            }
        }

        private async Task HandleReply(ToastNotificationActionTriggerDetail details, QueryString args)
        {
            // Get the conversation the toast is about
            string conversationId = args["conversationId"];

            // Get the message that the user typed in the toast
            string messagetext = (string)details.UserInput["Reply"];

            DiscordApiConfiguration config = new DiscordApiConfiguration
            {
                BaseUrl = "https://discordapp.com/api"
            };

            IAuthenticator authenticator = new DiscordAuthenticator(Storage.Token);
            AuthenticatedRestFactory authenticatedRestFactory = new AuthenticatedRestFactory(config, authenticator);

            SharedModels.GatewayConfig gateconfig = new SharedModels.GatewayConfig()
            {
                BaseUrl = "wss://gateway.discord.gg/"
            };

            Gateway.Gateway gateway = new Gateway.Gateway(gateconfig, authenticator);

            MessageUpsert message = new MessageUpsert();
            message.Content = messagetext;
            IChannelService channelservice = authenticatedRestFactory.GetChannelService();
            Task<SharedModels.Message> messageTask = channelservice.CreateMessage(conversationId, message);

            //message_task.Wait();


            // In a real app, you most likely should NOT notify your user that the request completed (only notify them if there's an error)
            //SendToast("Your message has been sent! Your message: " + message);

        }
    }
}
