// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;
using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Navigation;
using System;

namespace Quarrel.ViewModels.SubPages
{
    /// <summary>
    /// Change nickname sub page data.
    /// </summary>
    public class ChangeNicknameViewModel : ViewModelBase
    {
        private string _guildId;
        private GuildMember _member;
        private RelayCommand _saveCommand;
        private RelayCommand _backCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNicknameViewModel"/> class.
        /// </summary>
        public ChangeNicknameViewModel()
        {
            if (SubFrameNavigationService.Parameter != null)
            {
                Tuple<string, GuildMember> args = (Tuple<string, GuildMember>)SubFrameNavigationService.Parameter;
                _guildId = args.Item1;
                _member = args.Item2;

                Nickname = _member.Nick;
            }
        }

        /// <summary>
        /// Gets or sets the nickname of the member.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets the user's nickname.
        /// </summary>
        public string Username => _member.User.Username;

        /// <summary>
        /// Gets a command that updates the user's nickname.
        /// </summary>
        public RelayCommand SaveCommand => _saveCommand = _saveCommand ?? new RelayCommand(async () =>
        {
            IModifyGuildMember modifyGuildMember = new IModifyGuildMember() { Nick = Nickname };
            if (CurrentUserService.CurrentUser.Model.Id == _member.User.Id)
            {
                await SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.ModifySelfNickname(_guildId, modifyGuildMember);
            }
            else
            {
                // TODO: Modify other's usernames
                await SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.ModifyGuildMemberNickname(_guildId, _member.User.Id, modifyGuildMember);
            }

            SubFrameNavigationService.GoBack();
        });

        /// <summary>
        /// Gets a command that closes the page.
        /// </summary>
        public RelayCommand BackCommand => _backCommand = _backCommand ?? new RelayCommand(() =>
        {
            SubFrameNavigationService.GoBack();
        });

        private ICurrentUserService CurrentUserService { get; } = SimpleIoc.Default.GetInstance<ICurrentUserService>();
        private ISubFrameNavigationService SubFrameNavigationService { get; } = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();
    }
}
