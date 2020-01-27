using DiscordAPI.Models;
using Quarrel.Helpers.Colors.SmartColor;
using Quarrel.ViewModels.Services.DerivedColor;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.Services.DerivedColor
{
    public class ColorService : IColorService
    {
        SemaphoreSlim Mutex = new SemaphoreSlim(1, 1);

        public async Task<int> GetUserColor(User user)
        {
            await Mutex.WaitAsync();
            try
            {
                if (string.IsNullOrEmpty(user.Avatar)) return ColorExtensions.GetDiscriminatorColor(user.Discriminator).ToInt();

                // TODO: Handle avatar change
                if (userColorsCache.ContainsKey(user.Id)) return userColorsCache[user.Id];

                PictureAnalysis analysis = new PictureAnalysis();
                try
                {
                    await analysis.Analyse(new BitmapImage(user.AvatarUri), 128, 128);
                }
                catch { }
                if (analysis.ColorList.Count > 0)
                {
                    userColorsCache.Add(user.Id, analysis.ColorList[0].Color.ToInt());
                    return userColorsCache[user.Id];
                }
                else
                {
                    return ((Color)App.Current.Resources["BlurpleColor"]).ToInt();
                }
            }
            finally
            {
                Mutex.Release();
            }
        }

        public int GetStatusColor(string status)
        {
            return (App.Current.Resources[status ?? "offline"] as SolidColorBrush).Color.ToInt();
        }

        Dictionary<string, int> userColorsCache = new Dictionary<string, int>();
    }
}
