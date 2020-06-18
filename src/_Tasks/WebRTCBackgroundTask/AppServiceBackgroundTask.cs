using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Calls;
using Windows.Foundation.Collections;
using DiscordAPI.Models;
using DiscordAPI.Voice;
using DiscordAPI.Voice.DownstreamEvents;
using Newtonsoft.Json;
using Webrtc;

namespace WebRTCBackgroundTask
{
    public sealed class AppServiceBackgroundTask : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;
        private AppServiceConnection appServiceconnection;
        private static VoipPhoneCall voipCall;
        private static VoiceConnection voiceConnection;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCanceled;

            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            appServiceconnection = details.AppServiceConnection;
            appServiceconnection.RequestReceived += OnRequestReceived;
        }
        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            AppServiceDeferral deferral = args.GetDeferral();
            ValueSet message = args.Request.Message;

            

            switch (message["type"])
            {
                case "connect":
                    VoiceServerUpdate data = JsonConvert.DeserializeObject<VoiceServerUpdate>(message["config"] as string);
                    VoiceState state = JsonConvert.DeserializeObject<VoiceState>(message["state"] as string);
                    ConnectToVoiceChannel(data, state);
                    break;
            }
            deferral.Complete();
        }


        private async void ConnectToVoiceChannel(VoiceServerUpdate data, VoiceState state)
        {
            // TODO: More info for Mobile
            VoipCallCoordinator vcc = VoipCallCoordinator.GetDefault();
            var status = await vcc.ReserveCallResourcesAsync("WebRTCBackgroundTask.VoipBackgroundTask");
            voipCall = vcc.RequestNewOutgoingCall(string.Empty, string.Empty, "Quarrel", VoipPhoneCallMedia.Audio);
            voipCall.NotifyCallActive();

            voiceConnection = new VoiceConnection(data, state, new WebrtcManager("", ""));
            voiceConnection.Speak += Speak;
            await voiceConnection.ConnectAsync(); 
        }

        private void Speak(object sender, VoiceConnectionEventArgs<Speak> e)
        {
            // TODO: send speaking
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            deferral.Complete();
        }
    }
}
