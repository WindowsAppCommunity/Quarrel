using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels
{
    public partial class MainViewModel
    {
        #region Commands

        #region GuildSubscriptions

        private RelayCommand<(double, double)> updateGuildSubscriptionsCommand;
        public RelayCommand<(double, double)> UpdateGuildSubscriptionsCommand =>
            updateGuildSubscriptionsCommand ??= new RelayCommand<(double, double)>((values) =>
            {
                if (GuildsService.CurrentGuild.IsDM)
                    return;

                double top = CurrentBindableMembers.Count * values.Item1;
                double bottom = CurrentBindableMembers.Count * values.Item2;

                int min = (int)Math.Floor(top / 100) * 100;
                var guildSubscription = new Dictionary<string, IEnumerable<int[]>>
                {
                    {
                        CurrentChannel.Model.Id,
                        new List<int[]>
                        {
                            new[] { 0, 99 }
                        }
                    }
                };
                if (top - min < 20)
                {
                    if (min > 199)
                        ((List<int[]>)guildSubscription[CurrentChannel.Model.Id]).Add(new[] { min - 100, min - 1 });
                    if (min > 99)
                        ((List<int[]>)guildSubscription[CurrentChannel.Model.Id]).Add(new[] { min, min + 99 });
                }
                else if (bottom - min > 80)
                {
                    ((List<int[]>)guildSubscription[CurrentChannel.Model.Id]).Add(new[] { min, min + 99 });
                    ((List<int[]>)guildSubscription[CurrentChannel.Model.Id]).Add(new[] { min + 100, min + 199 });
                }
                else
                {
                    if (min > 99)
                        ((List<int[]>)guildSubscription[CurrentChannel.Model.Id]).Add(new[] { min, min + 99 });
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
                    Messenger.Default.Send(new GatewayUpdateGuildSubscriptionsMessage(guildId, guildSubscription));
                    lastGuildSubscription = guildSubscription;
                }

            });

        #endregion

        #endregion

        #region Methods

        private void RegisterMembersMessages()
        {
            #region Gateway

            #region Members

            // Handles VoiceState change for current user
            MessengerInstance.Register<GatewayVoiceStateUpdateMessage>(this, m =>
            {
                if (m.VoiceState.UserId == DiscordService.CurrentUser.Id)
                    DispatcherHelper.CheckBeginInvokeOnUi(() => VoiceState = m.VoiceState);
            });

            MessengerInstance.Register<GatewayGuildMemberListUpdatedMessage>(this, m =>
            {
                if (m.GuildMemberListUpdated.GuildId == guildId)
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        if (m.GuildMemberListUpdated.Id != listId &&
                            m.GuildMemberListUpdated.Operators.All(x => x.Op != "SYNC")) return;
                        if (m.GuildMemberListUpdated.Groups != null)
                        {
                            CurrentBindableMemeberGroups.Clear();
                            int totalMemberCount = 0;

                            foreach (Group group in m.GuildMemberListUpdated.Groups)
                            {
                                totalMemberCount += @group.Count + 1;
                                CurrentBindableMemeberGroups.Add(new BindableGuildMemberGroup(@group));
                            }

                            int listCount = CurrentBindableMembers.Count;
                            if (listCount < totalMemberCount)
                                CurrentBindableMembers.AddRange(
                                    Enumerable.Repeat<BindableGuildMember>(null, totalMemberCount - listCount));
                            else if (listCount > totalMemberCount)
                                for (int i = 0; i < listCount - totalMemberCount; i++)
                                    CurrentBindableMembers.RemoveAt(CurrentBindableMembers.Count - 1);
                        }

                        foreach (Operator op in m.GuildMemberListUpdated.Operators)
                            switch (op.Op)
                            {
                                case "SYNC":
                                    {
                                        listId = m.GuildMemberListUpdated.Id;
                                        int index = op.Range[0];
                                        foreach (SyncItem item in op.Items)
                                        {
                                            UpdateMemberListItem(index, item);
                                            index++;
                                        }
                                    }
                                    break;

                                case "INVALIDATE":
                                    {
                                        for (int i = op.Range[0]; i <= op.Range[1] && CurrentBindableMembers.Count < i; i++)
                                        {
                                            if (CurrentBindableMembers[i] != null)
                                                CurrentBindableMembers[i] = null;
                                        }
                                    }
                                    break;

                                case "INSERT":
                                    {
                                        if (op.Item?.Group != null)
                                            CurrentBindableMembers.Insert(op.Index,
                                                new BindableGuildMemberGroup(op.Item.Group));
                                        else
                                        {
                                            CurrentBindableMembers.Insert(op.Index, new BindableGuildMember(op.Item.Member)
                                            {
                                                GuildId = guildId,
                                                IsOwner = op.Item.Member.User.Id ==
                                                          GuildsService.AllGuilds[guildId].Model.OwnerId,
                                                Presence = op.Item.Member.Presence
                                            });
                                            PresenceService.UpdateUserPrecense(op.Item.Member.User.Id, op.Item.Member.Presence);
                                        }
                                    }
                                    break;

                                case "UPDATE":
                                    {
                                        UpdateMemberListItem(op.Index, op.Item);
                                    }
                                    ;
                                    break;

                                case "DELETE":
                                    {
                                        CurrentBindableMembers.RemoveAt(op.Index);
                                    }
                                    ;
                                    break;
                            }
                    });
            });

            #endregion

            #endregion
        }

        private void UpdateMemberListItem(int index, SyncItem item)
        {
            if (item.Group != null)
            {
                CurrentBindableMembers[index] = new BindableGuildMemberGroup(item.Group);
            }
            else if (item.Member != null)
            {
                BindableGuildMember bGuildMember = new BindableGuildMember(item.Member)
                {
                    GuildId = guildId,
                    IsOwner = item.Member.User.Id == GuildsService.AllGuilds[guildId].Model.OwnerId,
                    Presence = item.Member.Presence
                };
                CurrentBindableMembers[index] = bGuildMember;
                PresenceService.UpdateUserPrecense(item.Member.User.Id, item.Member.Presence);
            }
        }

        #endregion

        #region Properties

        private Dictionary<string, IEnumerable<int[]>> lastGuildSubscription;

        private string listId;

        [NotNull]
        public ObservableRangeCollection<IGuildMemberListItem> CurrentBindableMembers { get; set; } =
            new ObservableRangeCollection<IGuildMemberListItem>();

        [NotNull]
        public ObservableCollection<BindableGuildMemberGroup> CurrentBindableMemeberGroups { get; } =
            new ObservableCollection<BindableGuildMemberGroup>();
        #endregion
    }
}
