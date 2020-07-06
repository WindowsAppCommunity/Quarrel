// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// The ViewModel for all data throughout the app.
    /// </summary>
    public partial class MainViewModel
    {
        private RelayCommand<(double, double)> _updateGuildSubscriptionsCommand;
        private Dictionary<string, IEnumerable<int[]>> lastGuildSubscription;
        private string listId;

        /// <summary>
        /// Gets a command that updates the guild subscriptions.
        /// </summary>
        public RelayCommand<(double, double)> UpdateGuildSubscriptionsCommand =>
            _updateGuildSubscriptionsCommand = _updateGuildSubscriptionsCommand ?? new RelayCommand<(double, double)>((values) =>
            {
                Task.Run(() =>
                {
                    if (_guildsService.CurrentGuild.IsDM)
                    {
                        return;
                    }

                    double top = CurrentBindableMembers.Count * values.Item1;
                    double bottom = CurrentBindableMembers.Count * values.Item2;

                    int min = (int)Math.Floor(top / 100) * 100;
                    var guildSubscription = new Dictionary<string, IEnumerable<int[]>>
                    {
                        {
                            CurrentChannel.Model.Id,
                            new List<int[]>
                            {
                                new[] { 0, 99 },
                            }
                        },
                    };
                    if (top - min < 20)
                    {
                        if (min > 199)
                        {
                            ((List<int[]>)guildSubscription[CurrentChannel.Model.Id]).Add(new[] { min - 100, min - 1 });
                        }

                        if (min > 99)
                        {
                            ((List<int[]>)guildSubscription[CurrentChannel.Model.Id]).Add(new[] { min, min + 99 });
                        }
                    }
                    else if (bottom - min > 80)
                    {
                        ((List<int[]>)guildSubscription[CurrentChannel.Model.Id]).Add(new[] { min, min + 99 });
                        ((List<int[]>)guildSubscription[CurrentChannel.Model.Id]).Add(new[] { min + 100, min + 199 });
                    }
                    else
                    {
                        if (min > 99)
                        {
                            ((List<int[]>)guildSubscription[CurrentChannel.Model.Id]).Add(new[] { min, min + 99 });
                        }
                    }

                    bool hasChanged = false;

                    // Check if anything has changed
                    if (lastGuildSubscription != null && lastGuildSubscription.Count == guildSubscription.Count)
                    {
                        foreach (var channel in lastGuildSubscription)
                        {
                            if (guildSubscription.ContainsKey(channel.Key))
                            {
                                if (channel.Value.Count() == guildSubscription[channel.Key].Count())
                                {
                                    var enumerator = guildSubscription[channel.Key].GetEnumerator();
                                    foreach (var range in channel.Value)
                                    {
                                        enumerator.MoveNext();
                                        if (!(range[0] == enumerator.Current[0] && range[1] == enumerator.Current[1]))
                                        {
                                            hasChanged = true;
                                        }
                                    }
                                }
                                else
                                {
                                    hasChanged = true;
                                }
                            }
                            else
                            {
                                hasChanged = true;
                            }
                        }
                    }
                    else
                    {
                        hasChanged = true;
                    }

                    if (hasChanged)
                    {
                        Messenger.Default.Send(
                            new GatewayUpdateGuildSubscriptionsMessage(_currentGuild.Model.Id, guildSubscription));
                        lastGuildSubscription = guildSubscription;
                    }
                });
            });

        /// <summary>
        /// Gets list of members in the current guild.
        /// </summary>
        [NotNull]
        public ObservableRangeCollection<IGuildMemberListItem> CurrentBindableMembers { get; private set; } =
            new ObservableRangeCollection<IGuildMemberListItem>();

        /// <summary>
        /// Gets list of roles (Member Group).
        /// </summary>
        [NotNull]
        public ObservableCollection<BindableGuildMemberGroup> CurrentBindableMemeberGroups { get; } =
            new ObservableCollection<BindableGuildMemberGroup>();

        private void RegisterMembersMessages()
        {
            // Handles VoiceState change for current user
            MessengerInstance.Register<GatewayVoiceStateUpdateMessage>(this, m =>
            {
                if (m.VoiceState.UserId == _discordService.CurrentUser.Id)
                {
                    _dispatcherHelper.CheckBeginInvokeOnUi(() => VoiceState = m.VoiceState);
                }
            });

            MessengerInstance.Register<GatewayGuildMemberListUpdatedMessage>(this, m =>
            {
                if (m.GuildMemberListUpdated.GuildId == _currentGuild.Model.Id)
                {
                    string guildId = m.GuildMemberListUpdated.GuildId;
                    _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        if (m.GuildMemberListUpdated.Id != listId && m.GuildMemberListUpdated.Operators.All(x => x.Op != "SYNC"))
                        {
                            return;
                        }

                        if (m.GuildMemberListUpdated.Groups != null)
                        {
                            CurrentBindableMemeberGroups.Clear();
                            int totalMemberCount = 0;

                            foreach (Group group in m.GuildMemberListUpdated.Groups)
                            {
                                totalMemberCount += @group.Count + 1;
                                CurrentBindableMemeberGroups.Add(new BindableGuildMemberGroup(group, guildId));
                            }

                            int listCount = CurrentBindableMembers.Count;
                            if (listCount < totalMemberCount)
                            {
                                CurrentBindableMembers.AddRange(Enumerable.Repeat<BindableGuildMember>(null, totalMemberCount - listCount));
                            }
                            else if (listCount > totalMemberCount)
                            {
                                for (int i = 0; i < listCount - totalMemberCount; i++)
                                {
                                    CurrentBindableMembers.RemoveAt(CurrentBindableMembers.Count - 1);
                                }
                            }
                        }

                        foreach (Operator op in m.GuildMemberListUpdated.Operators)
                        {
                            switch (op.Op)
                            {
                                case "SYNC":
                                    {
                                        listId = m.GuildMemberListUpdated.Id;
                                        int index = op.Range[0];
                                        foreach (SyncItem item in op.Items)
                                        {
                                            UpdateMemberListItem(index, item, guildId);
                                            index++;
                                        }
                                    }

                                    break;

                                case "INVALIDATE":
                                    {
                                        for (int i = op.Range[0]; i <= op.Range[1] && CurrentBindableMembers.Count < i; i++)
                                        {
                                            CurrentBindableMembers[i] = null;
                                        }
                                    }

                                    break;

                                case "INSERT":
                                    {
                                        if (op.Item?.Group != null)
                                        {
                                            CurrentBindableMembers.Insert(op.Index, new BindableGuildMemberGroup(op.Item.Group, m.GuildMemberListUpdated.GuildId));
                                        }
                                        else
                                        {
                                            CurrentBindableMembers.Insert(op.Index, new BindableGuildMember(op.Item.Member, guildId)
                                            {
                                                IsOwner = op.Item.Member.User.Id == _guildsService.GetGuild(_currentGuild.Model.Id).Model.OwnerId,
                                            });
                                            _presenceService.UpdateUserPrecense(op.Item.Member.User.Id, op.Item.Member.Presence);
                                        }
                                    }

                                    break;

                                case "UPDATE":
                                    {
                                        UpdateMemberListItem(op.Index, op.Item, guildId);
                                    }

                                    break;

                                case "DELETE":
                                    {
                                        // TODO: Figure out why this must be checked
                                        if (op.Index < CurrentBindableMembers.Count)
                                        {
                                            CurrentBindableMembers.RemoveAt(op.Index);
                                        }
                                    }

                                    break;
                            }
                        }
                    });
                }
            });
        }

        private void UpdateMemberListItem(int index, SyncItem item, string guildId)
        {
            if (item.Group != null)
            {
                CurrentBindableMembers[index] = new BindableGuildMemberGroup(item.Group, guildId);
            }
            else if (item.Member != null)
            {
                BindableGuildMember bGuildMember = new BindableGuildMember(item.Member, _currentGuild.Model.Id)
                {
                    IsOwner = item.Member.User.Id == _guildsService.GetGuild(_currentGuild.Model.Id).Model.OwnerId,
                };
                CurrentBindableMembers[index] = bGuildMember;
                _presenceService.UpdateUserPrecense(item.Member.User.Id, item.Member.Presence);
            }
        }
    }
}
