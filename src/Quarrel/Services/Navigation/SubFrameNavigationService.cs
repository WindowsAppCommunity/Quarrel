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
    public class SubFrameNavigationService : ISubFrameNavigationService
    {
        private readonly List<(string Page, object Parameter)> _historic = new List<(string, object)>();
        private readonly ConcurrentDictionary<string, Type> _pagesByKey = new ConcurrentDictionary<string, Type>();
        public string CurrentPageKey { get; private set; }
        public object Parameter { get; private set; }

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

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            NavigateTo(pageKey, parameter, true);
        }

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

                DispatcherHelper.CheckBeginInvokeOnUI(() => Messenger.Default.Send(
                    SubFrameNavigationRequestMessage.To(
                        (UserControl) Activator.CreateInstance(_pagesByKey.TryGetValue(pageKey, out var pbk) ? pbk : null))));
            }
        }

        public void Configure(string key, Type pageType)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsKey(key))
                    _pagesByKey[key] = pageType;
                else
                    _pagesByKey.TryAdd(key, pageType);
            }
        }
    }
}