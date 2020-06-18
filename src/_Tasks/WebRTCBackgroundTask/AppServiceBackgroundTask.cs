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
        private static AppServiceConnection appServiceConnection;
        private static VoipPhoneCall voipCall;
        private static VoiceConnection voiceConnection;
        private static WebrtcManager webrtcManager;

        static AppServiceBackgroundTask()
        {
            webrtcManager = new WebrtcManager();
            webrtcManager.AudioInData += WebrtcManager_AudioInData;
            webrtcManager.AudioOutData += WebrtcManager_AudioOutData;
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCanceled;

            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            appServiceConnection = details.AppServiceConnection;
            appServiceConnection.RequestReceived += OnRequestReceived;
        }
        

        private static async void WebrtcManager_AudioInData(object sender, IList<float> e)
        {
            if (appServiceConnection == null) return;
            var message = new ValueSet()
            {
                ["type"] = "audioInData",
                ["data"] = e.ToArray(),
            };

            await appServiceConnection.SendMessageAsync(message);
        }

        private static async void WebrtcManager_AudioOutData(object sender, IList<float> e)
        {
            if (appServiceConnection == null) return;
            var message = new ValueSet()
            {
                ["type"] = "audioOutData",
                ["data"] = e.ToArray(),
            };

            await appServiceConnection.SendMessageAsync(message);
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            AppServiceDeferral deferral = args.GetDeferral();
            ValueSet message = args.Request.Message;

            switch (message["type"])
            {
                case "connect":
                    {
                        VoiceServerUpdate data = JsonConvert.DeserializeObject<VoiceServerUpdate>(message["config"] as string);
                        VoiceState state = JsonConvert.DeserializeObject<VoiceState>(message["state"] as string);
                        webrtcManager.SetRecordingDevice(message["inputId"] as string);
                        webrtcManager.SetPlaybackDevice(message["outputId"] as string);
                        ConnectToVoiceChannel(data, state);
                    }
                    break;
                case "disconnect":
                    webrtcManager.Destroy();
                    voipCall?.NotifyCallEnded();
                    voipCall = null;
                    voiceConnection = null;
                    break;
                case "voiceStateUpdate":
                    {
                        // Todo: handle here
                        VoiceState state = JsonConvert.DeserializeObject<VoiceState>(message["state"] as string);
                    }
                    break;
                case "inputDevice":
                    webrtcManager.SetRecordingDevice(message["id"] as string);
                    break;
                case "outputDevice":
                    webrtcManager.SetPlaybackDevice(message["id"] as string);
                    break;
            }
            deferral.Complete();
        }


        private async void ConnectToVoiceChannel(VoiceServerUpdate data, VoiceState state)
        {
            voipCall?.NotifyCallEnded();
            VoipCallCoordinator vcc = VoipCallCoordinator.GetDefault();
            var status = await vcc.ReserveCallResourcesAsync("WebRTCBackgroundTask.VoipBackgroundTask");
            // TODO: More info for Mobile
            voipCall = vcc.RequestNewOutgoingCall(string.Empty, string.Empty, "Quarrel", VoipPhoneCallMedia.Audio);
            voipCall.NotifyCallActive();

            voiceConnection = new VoiceConnection(data, state, webrtcManager);
            voiceConnection.Speak += Speak;
            await voiceConnection.ConnectAsync(); 
        }

        private async void Speak(object sender, VoiceConnectionEventArgs<Speak> e)
        {
            var message = new ValueSet()
            {
                ["type"] = "speaking",
                ["payload"] = JsonConvert.SerializeObject(e.EventData),
            };

            await appServiceConnection?.SendMessageAsync(message);
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            deferral.Complete();
        }
    }
}
