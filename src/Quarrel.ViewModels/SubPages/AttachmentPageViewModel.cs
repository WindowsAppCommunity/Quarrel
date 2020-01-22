using DiscordAPI.Interfaces;
using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.ViewModels.SubPages
{
    public class AttachmentPageViewModel : ViewModelBase
    {
        public AttachmentPageViewModel(IPreviewableImage image)
        {
            Image = image;
        }

        /// <summary>
        /// Image on Page
        /// </summary>
        public IPreviewableImage Image { get; }

        #region Display

        /// <summary>
        /// True if file information can be read from the Attachment
        /// </summary>
        public bool IsFile { get => Image is Attachment; }

        /// <summary>
        /// Bindable property for getting file metadata
        /// </summary>
        public Attachment AsFile { get => Image as Attachment; }

        #endregion
    }
}
