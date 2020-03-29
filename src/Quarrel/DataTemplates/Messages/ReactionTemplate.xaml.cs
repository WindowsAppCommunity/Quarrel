// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Discord.Rest;
using Windows.UI.Xaml.Controls.Primitives;

namespace Quarrel.DataTemplates.Messages
{
    /// <summary>
    /// A collection of Data Templates for Reaction displaying.
    /// </summary>
    public partial class ReactionTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactionTemplate"/> class.
        /// </summary>
        public ReactionTemplate()
        {
            this.InitializeComponent();
        }

        // TODO: move to main view model
        private async void ToggleReaction(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            BindableReaction reaction = (e.OriginalSource as ToggleButton).DataContext as BindableReaction;

            string reactionFullId = reaction.Model.Emoji.Name +
                (reaction.Model.Emoji.Id == null ?
                string.Empty :
                ":" + reaction.Model.Emoji.Id);

            // Already updated
            if (!reaction.Me)
            {
                await SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.DeleteReaction(reaction.Model.ChannelId, reaction.Model.MessageId, reactionFullId);
            }
            else
            {
                await SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.CreateReaction(reaction.Model.ChannelId, reaction.Model.MessageId, reactionFullId);
            }
        }
    }
}
