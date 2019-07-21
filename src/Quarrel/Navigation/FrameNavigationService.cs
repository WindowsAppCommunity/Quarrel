using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Navigation.SubFrame;

namespace Quarrel.Navigation
{
    public class SubFrameNavigationService : ISubFrameNavigationService
    {
        private readonly Dictionary<string, Type> _pagesByKey = new Dictionary<string, Type>();
        private readonly List<string> _historic = new List<string>();
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

                Messenger.Default.Send(
                    SubFrameNavigationRequestMessage.To(
                        (UserControl) Activator.CreateInstance(_pagesByKey[pageKey])));
            }
        }

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
                    _pagesByKey.Add(key, pageType);
                }
            }
        }

    }
}
