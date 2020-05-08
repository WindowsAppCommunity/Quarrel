// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.SubPages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages
{
    /// <summary>
    /// A sub page for creating an invite.
    /// </summary>
    public sealed partial class CreateInvitePage : UserControl, IConstrainedSubPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateInvitePage"/> class.
        /// </summary>
        public CreateInvitePage()
        {
            this.InitializeComponent();

            DataContext = new CreateInvitePageViewModel((string)SubFrameNavigationService.Parameter);
        }

        /// <summary>
        /// Gets the create invite page data.
        /// </summary>
        public CreateInvitePageViewModel ViewModel => DataContext as CreateInvitePageViewModel;

        /// <inheritdoc/>
        public double MaxExpandedWidth => 600;

        /// <inheritdoc/>
        public double MaxExpandedHeight => 300;

        private ISubFrameNavigationService SubFrameNavigationService { get; } = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();
    }
}
