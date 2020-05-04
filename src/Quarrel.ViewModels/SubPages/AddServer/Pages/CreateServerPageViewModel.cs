// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Navigation;

namespace Quarrel.ViewModels.SubPages.AddServer.Pages
{
    /// <summary>
    /// Create Server page data.
    /// </summary>
    public class CreateServerPageViewModel : ViewModelBase
    {
        private CreateGuild _guild;
        private RelayCommand _createServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateServerPageViewModel"/> class.
        /// </summary>
        public CreateServerPageViewModel()
        {
            _guild = new CreateGuild();
        }

        /// <summary>
        /// Gets a command that creates the drafted guild.
        /// </summary>
        public RelayCommand CreateServer => _createServer = new RelayCommand(async () =>
        {
            await DiscordService.GuildService.CreateGuild(_guild);
            SubFrameNavigationService.GoBack();
        });

        /// <summary>
        /// Gets or sets the name of the drafted guild.
        /// </summary>
        public string Name
        {
            get => _guild.Name;
            set => _guild.Name = value;
        }

        private IDiscordService DiscordService { get; } = SimpleIoc.Default.GetInstance<IDiscordService>();

        private ISubFrameNavigationService SubFrameNavigationService { get; } = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();

        /// <summary>
        /// Updates the icon for the guild.
        /// </summary>
        /// <param name="base64Icon">The new icon in base64 string format.</param>
        public void UpdateIcon(string base64Icon)
        {
            _guild.Base64Icon = base64Icon;
        }
    }
}
