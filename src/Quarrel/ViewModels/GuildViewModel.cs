using Quarrel.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.ViewModels
{
    public class GuildViewModel
    {
        public ObservableCollection<BindableGuild> Source { get; } = new ObservableCollection<BindableGuild>();
    }
}
