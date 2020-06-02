// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models.Messages.Embeds;
using Quarrel.ViewModels.Models.Bindables.Messages.Embeds;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Messages.Embeds
{
    /// <summary>
    /// Control to display YouTube embed.
    /// </summary>
    public sealed partial class YoutubeEmbedTemplate : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YoutubeEmbedTemplate"/> class.
        /// </summary>
        public YoutubeEmbedTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets the DataContext as a VideoEmbed.
        /// </summary>
        public BindableVideoEmbed ViewModel => new BindableVideoEmbed(DataContext as Embed);

        private void UserControl_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (mytubeEmbed != null)
            {
                mytubeEmbed.Dispose();
            }
        }
    }
}
