// Quarrel © 2022

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
            TargetViewModelType = targetViewModelType;
        }

        /// <summary>
        /// Gets the type of ViewModel for the target SubPage.
        /// </summary>
        public Type TargetViewModelType { get; }
    }
}
