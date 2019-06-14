using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Quarrel.Models.Bindables.Abstract;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Models
{
    public class BindableMessage : BindableModelBase<Message>
    {
        public BindableMessage([NotNull] Message model) : base(model) { }
    }
}
