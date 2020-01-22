using System;
using System.Collections.Generic;
using System.Text;
using DiscordAPI.API.Guild.Models;
using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Navigation;
using Quarrel.ViewModels.Services.Rest;

namespace Quarrel.ViewModels.SubPages
{
    public class AddChannelPageViewModel : ViewModelBase
    {
        #region Constructors

        public AddChannelPageViewModel()
        {
            if (SubFrameNavigationService.Parameter != null)
            {
                guild = (Guild)SubFrameNavigationService.Parameter;
            }
        }

        #endregion

        #region Properties

        #region Services

        private ISubFrameNavigationService SubFrameNavigationService { get; } =
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();

        #endregion

        public string ChannelName { get; set; }

        public string SelectedChannelType { get; set; } = "Text Channel";

        private Guild guild;

        #endregion

        #region Commands

        private RelayCommand createChannelCommand;
        public RelayCommand CreateChannelCommand =>
            createChannelCommand ??= new RelayCommand(() =>
            {
                CreateGuildChannel createChannel = null;

                switch (SelectedChannelType)
                {
                    case "Text Channel":
                    {
                        createChannel = new CreateGuildChannel {Type = 0};
                    }
                        break;

                    case "Voice Channel":
                    {
                        // Todo: Add options for bitrate and userlimit
                        createChannel = new CreateVoiceChannel {Type = 2, Bitrate= 64000, UserLimit = 0};
                    }
                        break;
                    default:
                        return;

                }


                createChannel.Name = ChannelName;

                SimpleIoc.Default.GetInstance<IDiscordService>().GuildService
                    .CreateGuildChannel(guild.Id, createChannel);
                SubFrameNavigationService.GoBack();
            });


        private RelayCommand backCommand;
        public RelayCommand BackCommand =>
            backCommand ??= new RelayCommand(() =>
            {
                SubFrameNavigationService.GoBack();
            });

        #endregion
    }
}
