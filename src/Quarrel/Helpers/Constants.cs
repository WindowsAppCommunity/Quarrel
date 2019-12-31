using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Quarrel.UWP.Helpers
{
    public static class Constants
    {
        public static class FromFile
        {
            public class SplashText
            {
                public SplashText(string raw)
                {
                    var items = raw.Split(new string[] { @"\,"}, StringSplitOptions.RemoveEmptyEntries);
                    Text = items[0];
                    if (items.Count() == 2)
                    {
                        Credit = items[1];
                    }
                }

                public string Text { get; set; } = "";

                public string Credit { get; set; } = "";
            }

            public static async Task<SplashText> GetRandomSplash()
            {
                // Load the list of Splash entries
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Data/SplashText.txt"));
                IList<string> rawSplashes = await FileIO.ReadLinesAsync(file);

                // Select item from list
                int i = ThreadSafeRandom.Instance.Next(rawSplashes.Count);
                return new SplashText(rawSplashes[i]);
            }
        }
    }
}
