// Copyright (c) Quarrel. All rights reserved.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Quarrel.SubPages
{
    /// <summary>
    /// Displays information about unhandled exception.
    /// </summary>
    public sealed partial class BSOD : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOD"/> class.
        /// </summary>
        public BSOD()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc />
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var exception = e.Parameter as UnhandledExceptionEventArgs;
            Message.Text = exception.Message;
        }
    }
}