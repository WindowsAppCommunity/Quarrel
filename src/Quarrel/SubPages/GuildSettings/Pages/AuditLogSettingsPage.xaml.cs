using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.SubPages.GuildSettings.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Quarrel.SubPages.GuildSettings.Pages
{
    public sealed partial class AuditLogSettingsPage : Page
    {
        public AuditLogSettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = new AuditLogSettingsPageViewModel(e.Parameter as BindableGuild);
        }

        public AuditLogSettingsPageViewModel ViewModel => DataContext as AuditLogSettingsPageViewModel;
    }
}
