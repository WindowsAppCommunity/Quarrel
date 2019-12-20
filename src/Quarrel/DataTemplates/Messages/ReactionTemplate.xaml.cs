using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Services.Rest;
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

        private void ToggleReaction(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Reaction reaction = (e.OriginalSource as ToggleButton).DataContext as Reaction;

            string reactionFullId = reaction.Emoji.Name +
                (reaction.Emoji.Id == null ?
                "" :
                ":" + reaction.Emoji.Id);

            if (reaction.Me)
            {
                reaction.Count--;
                SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.CreateReaction(reaction.ChannelId, reaction.MessageId, reactionFullId);
            } else
            {
                reaction.Count++;
                SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.DeleteReaction(reaction.ChannelId, reaction.MessageId, reactionFullId);
            }

            // TODO: BindableReactions for count increment
        }
    }
}
