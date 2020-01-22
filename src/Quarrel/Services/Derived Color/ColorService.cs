using DiscordAPI.Models;
using Quarrel.Helpers.Colors.SmartColor;
using Quarrel.ViewModels.Services.DerivedColor;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.Services.DerivedColor
{
    public class ColorService : IColorService
    {
        public async Task<int> GetUserColor(User user)
        {
            if (string.IsNullOrEmpty(user.Avatar)) return ColorExtensions.GetDiscriminatorColor(user.Discriminator).ToInt();

            // TODO: Handle avatar change
            if (userColorsCache.ContainsKey(user.Id)) return userColorsCache[user.Id];

            PictureAnalysis analysis = new PictureAnalysis();
            try
            {
                await analysis.Analyse(new BitmapImage(user.AvatarUri), 128, 128);
                if (analysis.ColorList.Count > 0)
                {
                    userColorsCache.Add(user.Id, analysis.ColorList[0].Color.ToInt());
                    return userColorsCache[user.Id];
                }
            }
            catch
            {
                userColorsCache.Add(user.Id, ((Color)App.Current.Resources["BlurpleColor"]).ToInt());
                return userColorsCache[user.Id];
            }

            return ((Color)App.Current.Resources["BlurpleColor"]).ToInt();
        }

        Dictionary<string, int> userColorsCache = new Dictionary<string, int>();
    }
}
