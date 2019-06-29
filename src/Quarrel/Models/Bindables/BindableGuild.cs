// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Helpers;
using Quarrel.Models.Bindables.Abstract;
using Quarrel.Models.Interfaces;
using Quarrel.Services;
using Quarrel.Services.Cache;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;

namespace Quarrel.Models.Bindables
{
    public class BindableGuild : BindableModelBase<Guild>, IGuild
    {
        public BindableGuild([NotNull] Guild model) : base(model)
        {
            _Channels = new List<BindableChannel>();
        }

        private List<BindableChannel> _Channels;

        public List<BindableChannel> Channels
        {
            get  => _Channels;
            set => Set(ref _Channels, value);
        }

        public bool IsDM
        {
            get => Model.Id == "DM";
        }

        #region Settings

        private int _Position;

        public int Position
        {
            get => _Position;
            set => Set(ref _Position, value);
        }


        private bool _Muted;

        public bool Muted
        {
            get => _Muted;
            set => Set(ref _Muted, value);
        }

        #endregion

        #region Icon

        public string DisplayText
        {
            get
            {
                if (IsDM) { return ""; }
                else
                {
                    return String.Concat(Model.Name.Split(' ').Select(s => StringInfo.GetNextTextElement(s, 0)).ToArray());
                }
            }
        }

        public string IconUrl
        {
            get { return "https://cdn.discordapp.com/icons/" + Model.Id + "/" + Model.Icon + ".png"; }
        }

        public Uri IconUri { get { return new Uri(IconUrl); } }
            
        public bool HasIcon { get { return !String.IsNullOrEmpty(Model.Icon); } }

        #endregion
    }
}
