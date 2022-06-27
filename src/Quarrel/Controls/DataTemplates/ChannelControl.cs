// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Input;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.DataTemplates
{
    public class ChannelControl : Control
    {
        private const string ReadState_Read = "Read";
        private const string ReadState_Unread = "Unread";
        private const string ReadState_Muted = "Muted";

        public static readonly DependencyProperty ChannelNameProperty =
            DependencyProperty.Register(nameof(ChannelName), typeof(string), typeof(ChannelControl), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(ChannelControl), new PropertyMetadata(false));
        
        public static readonly DependencyProperty IsUnreadProperty =
            DependencyProperty.Register(nameof(IsUnread), typeof(bool), typeof(ChannelControl), new PropertyMetadata(false, OnIsUnreadUpdated));
        
        public static readonly DependencyProperty MarkAsReadCommandProperty =
            DependencyProperty.Register(nameof(MarkAsReadCommand), typeof(RelayCommand), typeof(ChannelControl), new PropertyMetadata(null));
        
        public static readonly DependencyProperty CopyLinkCommandProperty =
            DependencyProperty.Register(nameof(CopyLinkCommand), typeof(RelayCommand), typeof(ChannelControl), new PropertyMetadata(null));
        
        public static readonly DependencyProperty CopyIdCommandProperty =
            DependencyProperty.Register(nameof(CopyIdCommand), typeof(RelayCommand), typeof(ChannelControl), new PropertyMetadata(null));
        
        public ChannelControl()
        {
            this.DefaultStyleKey = typeof(ChannelControl);
        }

        public string ChannelName
        {
            get => (string)GetValue(ChannelNameProperty);
            set => SetValue(ChannelNameProperty, value);
        }

        public bool IsUnread
        {
            get => (bool)GetValue(IsUnreadProperty);
            set => SetValue(IsUnreadProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public RelayCommand MarkAsReadCommand
        {
            get => (RelayCommand)GetValue(MarkAsReadCommandProperty);
            set => SetValue(MarkAsReadCommandProperty, value);
        }

        public RelayCommand CopyLinkCommand
        {
            get => (RelayCommand)GetValue(CopyLinkCommandProperty);
            set => SetValue(CopyLinkCommandProperty, value);
        }

        public RelayCommand CopyIdCommand
        {
            get => (RelayCommand)GetValue(CopyIdCommandProperty);
            set => SetValue(CopyIdCommandProperty, value);
        }

        private static void OnIsUnreadUpdated(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = (ChannelControl)sender;
            bool isUnread = (bool)args.NewValue;
            control.UpdateReadVisualState(isUnread);
        }

        private void UpdateReadVisualState(bool isUnread)
        {
            string state = ReadState_Read;

            // TODO: Handle muted
            if (isUnread)
            {
                state = ReadState_Unread;
            }

            VisualStateManager.GoToState(this, state, true);
        }
    }
}
