// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Enums.Users;
using Discord.API.Models.Json.Settings;
using Discord.API.Models.Json.Users;
using Quarrel.Client.Models.Settings;
using Quarrel.Client.Models.Users;
using System;
using System.Threading.Tasks;


namespace Quarrel.Client
{
    public partial class QuarrelClient
    {
        /// <summary>
        /// A class managing the <see cref="QuarrelClient"/>'s self user.
        /// </summary>
        public class QuarrelClientSelf
        {
            private readonly QuarrelClient _client;
            private SelfUser? _currentUser;

            public QuarrelClientSelf(QuarrelClient client)
            {
                _client = client;
            }

            /// <summary>
            /// Gets the current user.
            /// </summary>
            public SelfUser? CurrentUser { get; private set; }

            /// <summary>
            /// Gets the client's settings.
            /// </summary>
            public UserSettings? Settings { get; internal set; }

            /// <summary>
            /// Modifies the current user.
            /// </summary>
            /// <param name="modifyUser">The self user modifications.</param>
            public async Task ModifyMe(ModifySelfUser modifyUser)
            {
                Guard.IsNotNull(_client.UserService, nameof(UserService));

                await _client.UserService.ModifyMe(modifyUser.ToJsonModel());
            }

            /// <summary>
            /// Modifies user settings.
            /// </summary>
            /// <param name="modifySettings">The settings modifications.</param>
            public async Task ModifySettings(ModifyUserSettings modifySettings)
            {
                Guard.IsNotNull(_client.UserService, nameof(_client.UserService));

                await _client.UserService.UpdateSettings(modifySettings.ToJsonModel());
            }

            /// <summary>
            /// Updates the user's online status.
            /// </summary>
            /// <param name="status">The new online status to set.</param>
            public async Task UpdateStatus(UserStatus status)
            {
                Guard.IsNotNull(_client.Gateway, nameof(_client.Gateway));
                Guard.IsNotNull(_client.UserService, nameof(_client.UserService));

                await _client.Gateway.UpdateStatusAsync(status);
                var settingsUpdate = new JsonModifyUserSettings()
                {
                    Status = status.GetStringValue(),
                };
                await _client.UserService.UpdateSettings(settingsUpdate);
            }

            internal void SetSelfUser(JsonUser jsonUser)
            {
                CurrentUser = new SelfUser(jsonUser, _client);
                _client.Users.AddUser(CurrentUser);
            }

            internal void UpdateSettings(JsonUserSettings jsonUserSettings)
            {
                var settings = new UserSettings(jsonUserSettings, _client);
                Settings = settings;

                Guard.IsNotNull(CurrentUser, nameof(CurrentUser));

                CurrentUser.Presence = new Presence(new JsonPresence()
                {
                    Status = jsonUserSettings.Status,
                });
            }
        }
    }
}
