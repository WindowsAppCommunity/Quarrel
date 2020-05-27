// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;
using DiscordAPI.Models;
using DiscordAPI.Models.Guilds;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Navigation;

namespace Quarrel.ViewModels.SubPages
{
    /// <summary>
    /// Add Channel page data.
    /// </summary>
    public class AddChannelPageViewModel : ViewModelBase
    {
        private Guild _guild;
        private RelayCommand _backCommand;
        private RelayCommand _createChannelCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddChannelPageViewModel"/> class.
        /// </summary>
        public AddChannelPageViewModel()
        {
            if (SubFrameNavigationService.Parameter != null)
            {
                _guild = (Guild)SubFrameNavigationService.Parameter;
            }
        }

        /// <summary>
        /// Gets or sets the name of the drafted channel.
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// Gets or sets the type of the drafted channel.
        /// </summary>
        public string SelectedChannelType { get; set; } = "Text Channel";

        /// <summary>
        /// Gets a command that creates the drafted channel.
        /// </summary>
        public RelayCommand CreateChannelCommand => _createChannelCommand = _createChannelCommand ?? new RelayCommand(() =>
        {
            CreateGuildChannel createChannel = null;

            switch (SelectedChannelType)
            {
                case "Text Channel":
                    createChannel = new CreateGuildChannel { Type = 0 };
                    break;

                case "Voice Channel":
                    // TODO: Add options for bitrate and userlimit
                    createChannel = new CreateVoiceChannel { Type = 2, Bitrate = 64000, UserLimit = 0 };
                    break;
                default:
                    return;
            }

            createChannel.Name = ChannelName;

            SimpleIoc.Default.GetInstance<IDiscordService>().GuildService
                .CreateGuildChannel(_guild.Id, createChannel);
            SubFrameNavigationService.GoBack();
        });

        /// <summary>
        /// Gets a command that closes the page.
        /// </summary>
        public RelayCommand BackCommand => _backCommand = _backCommand ?? new RelayCommand(() =>
        {
            SubFrameNavigationService.GoBack();
        });

        private ISubFrameNavigationService SubFrameNavigationService { get; } = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();
    }
}
