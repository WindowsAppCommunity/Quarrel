using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Posts.Requests;
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
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Guild;
using Quarrel.ViewModels.Models.Bindables;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Guilds
{
    public class BindableMutualGuild
    {
        public IGuildsService GuildsService = SimpleIoc.Default.GetInstance<IGuildsService>();
        public BindableMutualGuild(MutualGuild mg)
        {
            MutualGuild = mg;
        }

        public MutualGuild MutualGuild { get; set; }

        public BindableGuild BindableGuild => GuildsService.Guilds.TryGetValue(MutualGuild.Id, out var value) ? value : null;
    }

    public sealed partial class MutualGuildTemplate : UserControl
    {
        public MutualGuildTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                if (e.NewValue is MutualGuild guild)
                    DataContext = new BindableMutualGuild(guild);
                else
                    this.Bindings.Update();
            };
        }

        BindableMutualGuild ViewModel => DataContext as BindableMutualGuild;
    }
}
