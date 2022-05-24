// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;

namespace Quarrel.Messages.Navigation.SubPages
{
    /// <summary>
    /// A message sent when navigation to a SubPage is requested.
    /// </summary>
    public class NavigateToSubPageMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateToSubPageMessage"/> class.
        /// </summary>
        public NavigateToSubPageMessage(Type targetViewModelType)
        {
            // TODO: Investigate alternative to using Ioc
            ViewModel = (ObservableObject)Ioc.Default.GetRequiredService(targetViewModelType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateToSubPageMessage"/> class.
        /// </summary>
        public NavigateToSubPageMessage(ObservableObject viewModel)
        {
            ViewModel = viewModel;
        }

        /// <summary>
        /// Gets the type of ViewModel for the target SubPage.
        /// </summary>
        public ObservableObject ViewModel { get; }
    }
}
