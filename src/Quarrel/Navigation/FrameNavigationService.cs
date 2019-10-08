using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Navigation.SubFrame;
using System.Collections.Concurrent;
using GalaSoft.MvvmLight.Threading;

namespace Quarrel.Navigation
{
    public class SubFrameNavigationService : ISubFrameNavigationService
    {
        private readonly List<string> _historic = new List<string>();
        private readonly ConcurrentDictionary<string, Type> _pagesByKey = new ConcurrentDictionary<string, Type>();
        public string CurrentPageKey { get; private set; }
        public object Parameter { get; private set; }

        public void GoBack()
        {
            if (_historic.Count > 1)
            {
                _historic.RemoveAt(_historic.Count - 1);
                NavigateTo(_historic.Last(), null, false);
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
                    _historic.Add(pageKey);
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