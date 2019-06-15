// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Models.Bindables.Abstract;

namespace Quarrel.Models.Bindables
{
    public class BindableGuild : BindableModelBase<Guild>
    {
        public BindableGuild([NotNull] Guild model) : base(model) { }
    }
}
