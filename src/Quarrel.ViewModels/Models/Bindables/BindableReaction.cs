using DiscordAPI.Models;
using Quarrel.Models.Bindables.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Models.Bindables
{
    public class BindableReaction : BindableModelBase<Reaction>
    {
        public BindableReaction(Reaction reaction) : base(reaction) { }

        public bool Me
        {
            get => Model.Me;
            set
            {
                Model.Me = value;
                RaisePropertyChanged(nameof(Me));
            }
        }

        public int Count
        {
            get => Model.Count;
            set
            {
                Model.Count = value;
                RaisePropertyChanged(nameof(Count));
            }
        }
    }
}
