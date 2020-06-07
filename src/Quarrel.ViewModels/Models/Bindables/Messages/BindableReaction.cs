// Copyright (c) Quarrel. All rights reserved.

using System.Collections.Generic;
using System.Text;
using DiscordAPI.Models;
using DiscordAPI.Models.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Services.Discord.Rest;

namespace Quarrel.ViewModels.Models.Bindables.Messages
{
    /// <summary>
    /// A Bindable wrapper of the <see cref="Reaction"/> model.
    /// </summary>
    public class BindableReaction : BindableModelBase<Reaction>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableReaction"/> class.
        /// </summary>
        /// <param name="reaction">The base <see cref="Reaction"/> object.</param>
        public BindableReaction(Reaction reaction) : base(reaction)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the current user has used this reaction.
        /// </summary>
        public bool Me
        {
            get => Model.Me;
            set
            {
                Model.Me = value;
                RaisePropertyChanged(nameof(Me));
                usersReacted = null;
            }
        }

        /// <summary>
        /// Gets or sets how many people have used this reaction.
        /// </summary>
        public int Count
        {
            get => Model.Count;
            set
            {
                Model.Count = value;
                RaisePropertyChanged(nameof(Count));
                usersReacted = null;
            }
        }

        private string _toolTip;

        /// <summary>
        /// Gets or sets the tooltip.
        /// </summary>
        public string ToolTip
        {
            get => _toolTip;
            set => Set(ref _toolTip, value);
        }

        private IEnumerable<User> usersReacted;

        private RelayCommand _updateToolTipCommand;

        public RelayCommand UpdateToolTipCommand => _updateToolTipCommand = _updateToolTipCommand ?? new RelayCommand(async () =>
        {
            if (usersReacted == null)
            {
                string reactionFullId = Model.Emoji.Name +  (Model.Emoji.Id == null ? string.Empty : ":" + Model.Emoji.Id);
                var channelService = SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService;
                usersReacted = await channelService.GetReactions(Model.ChannelId, Model.MessageId, reactionFullId);
            }

            StringBuilder sb = new StringBuilder();

            int total = 0;
            int count = 0;

            foreach (var user in usersReacted)
            {
                total++;
                if (count < 10)
                {
                    count++;
                    if (total == 1)
                    {
                        sb.Append(user.Username);
                    }
                    else
                    {
                        sb.Append(", ").Append(user.Username);
                    }
                }
            }

            if (total != count)
            {
                sb.Append(" and ").Append(total - count).Append(" others");
            }

            sb.Append(" reacted");
            // Todo: figure out how to find typeable emoji
            ToolTip = sb.ToString();
        });
    }
}
