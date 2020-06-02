// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models.Messages.Embeds;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Services.Settings;

namespace Quarrel.ViewModels.Models.Bindables.Messages.Embeds
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="Attachment"/> model.
    /// </summary>
    public class BindableAttachment : BindableModelBase<Attachment>
    {
        private bool _isShowing;
        private RelayCommand _showAttachmentCommand;
        private ISettingsService _settingsService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableAttachment"/> class.
        /// </summary>
        /// <param name="model">The base <see cref="Attachment"/> object.</param>
        public BindableAttachment(Attachment model) : base(model)
        {
        }

        /// <summary>
        /// Gets a command that shows the attachment.
        /// </summary>
        public RelayCommand ShowAttachmentCommand => _showAttachmentCommand = new RelayCommand(() =>
        {
            IsShowing = true;
        });

        /// <summary>
        /// Gets or sets a value indicating whether or not the attachment is showing.
        /// </summary>
        public bool IsShowing
        {
            get => !SettingsService.Roaming.GetValue<bool>(SettingKeys.TTLAttachments, true) || _isShowing;
            set => Set(ref _isShowing, value);
        }

        private ISettingsService SettingsService => _settingsService ?? (_settingsService = SimpleIoc.Default.GetInstance<ISettingsService>());
    }
}
