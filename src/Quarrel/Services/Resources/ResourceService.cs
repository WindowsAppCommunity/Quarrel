// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Quarrel.ViewModels.Services.Resources;
using QuarrelSmartColor;
using QuarrelSmartColor.Extensions.Windows.UI;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.Services.Resources
{
    /// <summary>
    /// Forwards resources through MVVM.
    /// </summary>
    public class ResourceService : IResourceService
    {
        private SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        private Dictionary<string, int> _userColorsCache = new Dictionary<string, int>();

        /// <inheritdoc/>
        public object GetResource(string resource)
        {
            return App.Current.Resources[resource];
        }

        /// <inheritdoc/>
        public async Task<int> GetUserAccentColor(User user)
        {
            await _mutex.WaitAsync();
            try
            {
                if (string.IsNullOrEmpty(user.Avatar))
                {
                    return ColorExtensions.GetDiscriminatorColor(user.Discriminator).ToInt();
                }

                // TODO: Handle avatar change
                if (_userColorsCache.ContainsKey(user.Id))
                {
                    return _userColorsCache[user.Id];
                }

                PictureAnalysis analysis = new PictureAnalysis();
                try
                {
                    analysis.Analyse(new BitmapImage(user.AvatarUri), 128, 128);
                }
                catch
                {
                }

                if (analysis.ColorList.Count > 0)
                {
                    _userColorsCache.Add(user.Id, analysis.ColorList[0].Color.ToInt());
                    return _userColorsCache[user.Id];
                }
                else
                {
                    return ((Color)App.Current.Resources["BlurpleColor"]).ToInt();
                }
            }
            finally
            {
                _mutex.Release();
            }
        }

        /// <inheritdoc/>
        public int GetStatusColor(string status)
        {
            return (App.Current.Resources[status ?? "offline"] as SolidColorBrush).Color.ToInt();
        }
    }
}
