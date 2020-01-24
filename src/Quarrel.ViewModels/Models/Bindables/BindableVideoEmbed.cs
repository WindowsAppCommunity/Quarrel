using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using Quarrel.ViewModels.Models.Bindables.Abstract;

namespace Quarrel.ViewModels.Models.Bindables
{
    public class BindableVideoEmbed : BindableModelBase<Embed>
    {
        #region Constructors

        public BindableVideoEmbed([NotNull] Embed model) : base(model) { }

        #endregion

        #region Properties

        private bool playingVideo;
        public bool PlayingVideo
        {
            get => playingVideo;
            set
            {
                Set(ref playingVideo, value);
                RaisePropertyChanged(nameof(NotPlayingVideo));
            }
        }
        public bool NotPlayingVideo
        {
            get => !playingVideo;
            set
            {
                Set(ref playingVideo, !value);
                RaisePropertyChanged(nameof(PlayingVideo));
            }
        }

        #endregion

        #region Commands

        private RelayCommand playVideoCommand;
        public RelayCommand PlayVideoCommand => playVideoCommand ?? (playVideoCommand = new RelayCommand(() =>
         {
             PlayingVideo = true;
         }));
        
        #endregion
    }
}
