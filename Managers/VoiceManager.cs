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

        private static bool hasSentSpeeking;
        private static bool stopSpeaking;
        private static readonly object syncLock = new object();
        public static event EventHandler<ConnectToVoiceArgs> ConnectoToVoiceHandler;

        public static async void ConnectToVoiceChannel(SharedModels.VoiceServerUpdate data)
        {
            App.UpdateLocalDeaf(false);
            App.UpdateVoiceStateHandler += App_UpdateVoiceStateHandler;
            //App.UpdateLocalMute(true); //LocalState.Muted);
            VoiceConnection = new VoiceConnection(data, LocalState.VoiceState);
            VoiceConnection.VoiceDataRecieved += VoiceConnection_VoiceDataRecieved;
            await VoiceConnection.ConnectAsync();

            ConnectoToVoiceHandler?.Invoke(typeof(App), new ConnectToVoiceArgs() { ChannelId = LocalState.VoiceState.ChannelId, GuildId = data.GuildId });
            
            AudioManager.InputRecieved += AudioManager_InputRecieved;
        }

        private static async void StopSpeaking()
        {
            stopSpeaking = true;
            if (!muted)
            {
                await Task.Delay(2000);
            }
            lock (syncLock)
            {
                if (stopSpeaking)
                {
                    VoiceConnection.SendSpeaking(false);
                    hasSentSpeeking = false;
                    stopSpeaking = false;
                }
            }

        }
        private static void App_UpdateVoiceStateHandler(object sender, EventArgs e)
        {
            muted = LocalState.VoiceState.SelfMute;
        }

        private static void AudioManager_InputRecieved(object sender, float[] e)
        {
            double decibels = 0f;
            foreach (var sample in e)
            {
                decibels += Math.Abs(sample);
            }
            decibels = 20 * Math.Log10(decibels / e.Length);
            if (decibels < Storage.Settings.NoiseSensitivity)
            {
                if (hasSentSpeeking && !stopSpeaking)
                {
                    StopSpeaking();
                }
            }
            else
            {
                if (!muted)
                {
                    stopSpeaking = false;
                    if (!hasSentSpeeking)
                    {
                        VoiceConnection.SendSpeaking(true);
                        hasSentSpeeking = true;
                    }
                    VoiceConnection.SendVoiceData(e);
                }
            }
        }

        private static void VoiceConnection_VoiceDataRecieved(object sender, VoiceConnectionEventArgs<Voice.DownstreamEvents.VoiceData> e)
        {
            AudioManager.AddFrame(e.EventData.data, e.EventData.samples);
        }



        public static VoiceConnection VoiceConnection;
        public static bool muted = false;
    }
}
