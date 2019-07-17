using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Models.Bindables;
using Quarrel.Services.Cache;

namespace Quarrel.Services.Users
{
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                TValue oldValue;
                bool exist = TryGetValue(key, out oldValue);
                var oldItem = new KeyValuePair<TKey, TValue>(key, oldValue);
                base[key] = value;
                var newItem = new KeyValuePair<TKey, TValue>(key, value);
                if (exist)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, Keys.ToList().IndexOf(key)));
                }
                else
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, Keys.ToList().IndexOf(key)));
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                }
            }
        }
        public new void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                var item = new KeyValuePair<TKey, TValue>(key, value);
                base.Add(key, value);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, Keys.ToList().IndexOf(key)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            }
        }
        public new bool Remove(TKey key)
        {
            if (TryGetValue(key, out TValue _))
            {
                KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, base[key]);
                bool result = base.Remove(key);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, Keys.ToList().IndexOf(key)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                return result;
            }
            return false;
        }
        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        }
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

    }


    public class CurrentUsersService : ICurrentUsersService
    {
        public ICacheService CacheService;

        public ObservableDictionary<string, BindableGuildMember> Users { get; } = new ObservableDictionary<string, BindableGuildMember>();

        public BindableUser CurrentUser { get; } = new BindableUser(new User());
        public string SessionId { get; set; }

        public CurrentUsersService(ICacheService cacheService)
        {
            CacheService = cacheService;
            Messenger.Default.Register<GatewayGuildSyncMessage>(this, m =>
            {

                // Show members
                Users.Clear();
                foreach (var member in m.Members)
                {
                    BindableGuildMember bGuildMember = new BindableGuildMember(member)
                    {
                        GuildId = m.GuildId,
                        Presence = Messenger.Default.Request<PresenceRequestMessage, Presence>(new PresenceRequestMessage(member.User.Id))
                    };
                    Users.Add(member.User.Id, bGuildMember);
                }

            });
            Messenger.Default.Register<GatewayReadyMessage>(this, async m =>
            {
                SessionId = m.EventData.SessionId;
                await DispatcherHelper.RunAsync(() => {
                    CurrentUser.Model = m.EventData.User;
                    CurrentUser.Presence = m.EventData.Presences.LastOrDefault();
                });
            });


            Messenger.Default.Register<GatewayPresenceUpdatedMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    if (CurrentUser.Model.Id == m.UserId)
                    {
                        CurrentUser.Presence = m.Presence;
                    }

                    Users.TryGetValue(m.UserId, out BindableGuildMember member);
                    if(member != null)
                    {
                        member.Presence = m.Presence;
                    }
                });
            });

            Messenger.Default.Register<GatewayUserSettingsUpdatedMessage>(this, async m =>
            {
                if (string.IsNullOrEmpty(m.Settings.Status))
                {
                    var newPresence = new Presence()
                    {
                        User = CurrentUser.Presence.User,
                        Game = CurrentUser.Presence.Game,
                        GuildId = CurrentUser.Presence.GuildId,
                        Roles = CurrentUser.Presence.Roles,
                        Status = m.Settings.Status
                    };
                    await DispatcherHelper.RunAsync(() =>
                    {

                        CurrentUser.Presence = newPresence;

                        Users.TryGetValue(CurrentUser.Model.Id, out BindableGuildMember member);
                        if (member != null)
                        {
                            member.Presence = newPresence;
                        }
                    });
                }
            });

            Messenger.Default.Register<GatewaySessionReplacedMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    var session = m.Session.FirstOrDefault(x => x.SessionId == SessionId);
                    var newPresence = new Presence()
                    {
                        User = CurrentUser.Presence.User,
                        Game = session.Game,
                        GuildId = CurrentUser.Presence.GuildId,
                        Roles = CurrentUser.Presence.Roles,
                        Status = session.Status
                    };
                    CurrentUser.Presence = newPresence;

                    Users.TryGetValue(CurrentUser.Model.Id, out BindableGuildMember member);
                    if (member != null)
                    {
                        member.Presence = newPresence;
                    }
                });
            });

        }
    }
}
