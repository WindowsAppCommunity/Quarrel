// Copyright (c) Quarrel. All rights reserved.

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
    /// <summary>
    /// A <see langword="class"/> that provides color parsing with Windows namespaces.
    /// </summary>
    public class ColorService : IColorService
    {
        private SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Remembers aleady calculated user colors.
        /// </summary>
        private Dictionary<string, int> _userColorsCache = new Dictionary<string, int>();

        /// <summary>
        /// Gets an int form color for a user based on their discriminator and avatar.
        /// </summary>
        /// <param name="user">User color is for.</param>
        /// <returns>An int form color.</returns>
        public async Task<int> GetUserColor(User user)
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
                    await analysis.Analyse(new BitmapImage(user.AvatarUri), 128, 128);
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

        /// <summary>
        /// Gets an int form color based on a user's status.
        /// </summary>
        /// <param name="status">User's status.</param>
        /// <returns>An int color.</returns>
        public int GetStatusColor(string status)
        {
            return (App.Current.Resources[status ?? "offline"] as SolidColorBrush).Color.ToInt();
        }
    }
}
