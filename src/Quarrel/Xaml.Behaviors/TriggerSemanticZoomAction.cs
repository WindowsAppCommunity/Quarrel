// Copyright (c) Quarrel. All rights reserved.

using Microsoft.Xaml.Interactivity;
using Quarrel.ViewModels.Models.Bindables.Users;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Xaml.Behaviors
{
    /// <summary>
    /// Action to toggle semantic zoom view conditionally based on clicked <see cref="ListViewItem"/> type in <see cref="ListView"/>.
    /// </summary>
    /// <remarks>Currently hardcoded to <see cref="BindableGuildMemberGroup"/> condition.</remarks>
    public class TriggerSemanticZoomAction : DependencyObject, IAction
    {
        /// <summary>
        /// Gets or sets the <see cref="Windows.UI.Xaml.Controls.SemanticZoom"/> to toggle when invoked.
        /// </summary>
        public SemanticZoom SemanticZoom { get; set; }

        /// <summary>
        /// Toggles the <see cref="SemanticZoom"/> view.
        /// </summary>
        /// <param name="sender">The <see cref="ListView"/> the <see cref="ListViewItem"/> as clicked in.</param>
        /// <param name="parameter">The <see cref="ItemClickEventArgs"/> to determine clicked item.</param>
        /// <returns><see langword="null"/>.</returns>
        public object Execute(object sender, object parameter)
        {
            // Should make Type dynamic
            // However it caused errors in release mode
            if ((parameter as ItemClickEventArgs)?.ClickedItem?.GetType() == typeof(BindableGuildMemberGroup))
            {
                SemanticZoom.IsZoomedInViewActive = false;
            }

            return null;
        }
    }
}
