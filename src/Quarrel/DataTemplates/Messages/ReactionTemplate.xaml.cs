using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Services.Rest;
using Quarrel.ViewModels.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;

namespace Quarrel.DataTemplates.Messages
{
    public partial class ReactionTemplate
    {
        public ReactionTemplate()
        {
            this.InitializeComponent();
        }

        private async void ToggleReaction(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            BindableReaction reaction = (e.OriginalSource as ToggleButton).DataContext as BindableReaction;

            string reactionFullId = reaction.Model.Emoji.Name +
                (reaction.Model.Emoji.Id == null ?
                "" :
                ":" + reaction.Model.Emoji.Id);

            // Already updated
            if (!reaction.Me)
            {
                await SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.DeleteReaction(reaction.Model.ChannelId, reaction.Model.MessageId, reactionFullId);
            } else
            {
                await SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.CreateReaction(reaction.Model.ChannelId, reaction.Model.MessageId, reactionFullId);
            }
        }
    }
}
