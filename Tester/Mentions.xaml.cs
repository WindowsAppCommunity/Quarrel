using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tester
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Mentions : Page
    {
        public Mentions()
        {
            this.InitializeComponent();
        }

        public List<Tuple<string, string>> FindMentions(string message)
        {
            List<Tuple<string, string>> mentions = new List<Tuple<string, string>>();
            bool inMention = false;
            bool inDesc = false;
            string cache = "";
            string descCache = "";
            foreach (char c in message)
            {
                if (inMention)
                {
                    if (c == '#' && !inDesc)
                    {
                        inDesc = true;
                    } else if (c == '@')
                    {
                        inDesc = false;
                        cache = "";
                        descCache = "";
                    } else if (inDesc)
                    {
                        if (Char.IsDigit(c))
                        {
                            descCache += c;
                        } else
                        {
                            inMention = false;
                            inDesc = false;
                            cache = "";
                            descCache = "";
                        }
                        if (descCache.Length == 4)
                        {
                            mentions.Add(new Tuple<string, string>(cache, descCache));
                            inMention = false;
                            inDesc = false;
                            cache = "";
                            descCache = "";
                        }
                    } else
                    {
                        cache += c;
                    }
                } else if (c == '@')
                {
                    inMention = true;
                }
            }
            return mentions;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mentions = FindMentions(Content.Text);
        }
    }
}
