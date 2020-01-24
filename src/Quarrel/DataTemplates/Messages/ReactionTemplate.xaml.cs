using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Rest;
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
