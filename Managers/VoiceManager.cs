using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord_UWP.Voice;
using Discord_UWP.LocalModels;

namespace Discord_UWP.Managers
{
    public static class VoiceManager
    {
        public class ConnectToVoiceArgs : EventArgs
        {
            public string ChannelId { get; set; }
            public string GuildId { get; set; }
        }
        public static event EventHandler<ConnectToVoiceArgs> ConnectoToVoiceHandler;

        public static async void ConnectToVoiceChannel(SharedModels.VoiceServerUpdate data)
        {
            VoiceConnection = new VoiceConnection(data, LocalState.VoiceState);
            VoiceConnection.VoiceDataRecieved += VoiceConnection_VoiceDataRecieved;
            await VoiceConnection.ConnectAsync();

            ConnectoToVoiceHandler?.Invoke(typeof(App), new ConnectToVoiceArgs() { ChannelId = LocalState.VoiceState.ChannelId, GuildId = data.GuildId });

            AudioManager.InputRecieved += AudioManager_InputRecieved;
        }

        private static void AudioManager_InputRecieved(object sender, float[] e)
        {
            //TODO: Sending voice
            //TODO: silence detection
            //VoiceConnection.SendSpeaking(true);
            //VoiceConnection.SendVoiceData(e);
        }

        private static void VoiceConnection_VoiceDataRecieved(object sender, VoiceConnectionEventArgs<Voice.DownstreamEvents.VoiceData> e)
        {
            AudioManager.AddFrame(e.EventData.data, e.EventData.samples);
        }

        public static VoiceConnection VoiceConnection;
    }
}
