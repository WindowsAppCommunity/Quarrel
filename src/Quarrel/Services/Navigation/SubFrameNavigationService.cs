// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.ViewModels.Messages.Navigation.SubFrame;
using Quarrel.ViewModels.Services.Navigation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Navigation
{
    /// <summary>
    /// A <see langword="class"/> that provides the ability to control the subframe.
    /// </summary>
    public class SubFrameNavigationService : ISubFrameNavigationService
    {
        private readonly List<(string Page, object Parameter)> _historic = new List<(string, object)>();
        private readonly ConcurrentDictionary<string, Type> _pagesByKey = new ConcurrentDictionary<string, Type>();

        /// <inheritdoc/>
        public string CurrentPageKey { get; private set; }

        /// <inheritdoc/>
        public object Parameter { get; private set; }

        /// <inheritdoc/>
        public int Depth => _historic.Count;

        /// <summary>
        /// Goes back a subpage in history or closes.
        /// </summary>
        public void GoBack()
        {
            if (_historic.Count > 1)
            {
                _historic.RemoveAt(_historic.Count - 1);
                var previous = _historic.Last();
                NavigateTo(previous.Page, previous.Parameter, false);
            }
            else
            {
                Messenger.Default.Send(new SubFrameCloseRequestMessage());
                _historic.Clear();
            }
        }

        /// <summary>
        /// Opens a subpage by key.
        /// </summary>
        /// <param name="pageKey">Which subpage to open.</param>
        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        /// <summary>
        /// Opens a subpage by key.
        /// </summary>
        /// <param name="pageKey">Which subpage to open.</param>
        /// <param name="parameter">Extra info for the subpage.</param>
        public void NavigateTo(string pageKey, object parameter)
        {
            NavigateTo(pageKey, parameter, true);
        }

        /// <summary>
        /// Opens a subpage by key.
        /// </summary>
        /// <param name="pageKey">Which subpage to open.</param>
        /// <param name="parameter">Extra info for the subpage.</param>
        /// <param name="addToHistory">Indicates if the subpage should be added to history.</param>
        public void NavigateTo(string pageKey, object parameter, bool addToHistory)
        {
            lock (_pagesByKey)
            {
                if (addToHistory)
                {
                    Parameter = parameter;
                    _historic.Add((pageKey, parameter));
                    CurrentPageKey = pageKey;
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Messenger.Default.Send(SubFrameNavigationRequestMessage.To((UserControl)Activator.CreateInstance(_pagesByKey.TryGetValue(pageKey, out var pbk) ? pbk : null)));
                });
            }
        }

        /// <summary>
        /// Adds subpages by their key.
        /// </summary>
        /// <param name="key">Key for the new subpage.</param>
        /// <param name="pageType">New subpage type.</param>
        public void Configure(string key, Type pageType)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsKey(key))
                {
                    _pagesByKey[key] = pageType;
                }
                else
                {
                    _pagesByKey.TryAdd(key, pageType);
                }
            }
        }
    }
}