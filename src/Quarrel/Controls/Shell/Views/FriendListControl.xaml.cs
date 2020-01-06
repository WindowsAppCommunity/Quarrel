using GalaSoft.MvvmLight.Ioc;
using Quarrel.Services.Users;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell.Views
{
    public sealed partial class FriendListControl : UserControl
    {
        public FriendListControl()
        {
            this.InitializeComponent();
        }

        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
