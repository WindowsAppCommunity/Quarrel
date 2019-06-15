using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Models.Bindables;
using Quarrel.Messages.Gateway;

namespace Quarrel.ViewModels
{
    public class GuildViewModel
    {
        public GuildViewModel()
        {

        }

        public ObservableCollection<BindableGuild> Source { get; } = new ObservableCollection<BindableGuild>();
    }
}
