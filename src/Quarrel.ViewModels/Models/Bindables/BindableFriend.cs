using DiscordAPI.Models;
using JetBrains.Annotations;
using Quarrel.Models.Bindables.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Models.Bindables
{
    public class BindableFriend : BindableModelBase<Friend>
    {
        public BindableFriend([NotNull] Friend friend) : base(friend) { }
    }
}
