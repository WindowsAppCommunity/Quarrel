using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DiscordAPI.API.User;
using Quarrel.Models.Bindables;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Posts.Requests;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Services.Users;

namespace Quarrel.ViewModels
{

    public class MemberViewModel : ViewModelBase
    {
        ICurrentUsersService currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUsersService>();

        public MemberViewModel()
        {
            currentUsersService.Users.CollectionChanged += CollectionChangedMethod;

          /*      Messenger.Default.Register<GatewayGuildSyncMessage>(this, async m =>
                {
                    var tempSource = new ObservableGroupCollection< Role, BindableGuildMember>();
                    await DispatcherHelper.RunAsync(() =>
                    {
                        Source.Clear();
                        // Show members
                        foreach (var member in m.Members)
                        {
                            BindableGuildMember bGuildMember = new BindableGuildMember(member);
                            bGuildMember.GuildId = m.GuildId;
                            /*  var tmp = Source.FirstOrDefault(x => x.Key.Equals(bGuildMember.TopHoistRole));
                              if (tmp != null)
                              {
                                  tmp.Add(bGuildMember);
                              }
                              else
                              {
                                  var bob = new ObservableGroupCollection<Role, BindableGuildMember>
                                  {
                                      Key = bGuildMember.TopHoistRole
                                  };
                                  bob.Add(bGuildMember);
                                  Source.Add(bob);
                              }
                            Source.AddElement(bGuildMember);

                            //  tempSource.AddElement(member.User.Id, bGuildMember);
                        }
                    });

                    //   await DispatcherHelper.RunAsync(() => { Source = tempSource; RaisePropertyChanged(nameof(Source)); });
                });*/

        }

        private void CollectionChangedMethod(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    _ = DispatcherHelper.RunAsync(() =>
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is KeyValuePair<string, BindableGuildMember> member)
                            {
                                Source.AddElement(member.Value);
                            }
                        }

                    });

                    break;
                }

                case NotifyCollectionChangedAction.Replace:
                {
                    _ = DispatcherHelper.RunAsync(() =>
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is KeyValuePair<string, BindableGuildMember> member)
                            {
                                Source.RemoveElement(member.Value);
                            }
                        }

                        foreach (var item in e.NewItems)
                        {
                            if (item is KeyValuePair<string, BindableGuildMember> member)
                            {
                                Source.AddElement(member.Value);
                            }
                        }
                    });

                    break;
                }

                case NotifyCollectionChangedAction.Remove:
                {
                    _ = DispatcherHelper.RunAsync(() =>
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is BindableGuildMember member)
                            {
                                Source.RemoveElement(member);
                            }
                        }
                    }); 

                    break;
                }

                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                {
                    _ = DispatcherHelper.RunAsync(() =>
                    {
                        //Note: reset must only be called from clear or this will not work

                        Source.Clear();
                    });

                    break;
                }
            }
        }

        /// <summary>
        /// Gets the collection of grouped feeds to display
        /// </summary>
        [NotNull]
        /*public SortedGroupedObservableHashedCollection<string, Role, BindableGuildMember> Source { get; set; } =
             new SortedGroupedObservableHashedCollection<string, Role, BindableGuildMember>(x => x.TopHoistRole,
                 x => -x.Key.Position,
                 new List<KeyValuePair<string, HashedGrouping<string, Role, BindableGuildMember>>>());*/
        public ObservableSortedGroupedCollection<Role, BindableGuildMember> Source { get; set; } = new ObservableSortedGroupedCollection<Role, BindableGuildMember>( x => x.TopHoistRole, x => -x.Position);
    }
}
