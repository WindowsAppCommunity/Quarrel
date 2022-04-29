// Quarrel © 2022

using Quarrel.ViewModels.Enums;

namespace Quarrel.Controls.Shell.Enums
{
    /// <summary>
    /// An enum for the state of the splash screen.
    /// </summary>
    /// <remarks>
    /// Values align with <see cref="WindowHostState"/>.
    /// </remarks>
    public enum SplashStatus
    {
        /// <inheritdoc cref="WindowHostState.Loading"/>
        Loading = WindowHostState.Loading,

        /// <inheritdoc cref="WindowHostState.Connecting"/>
        Connecting = WindowHostState.Connecting,
        
        /// <summary>
        /// Connected and playing the finishing animation.
        /// </summary>
        Connected = WindowHostState.LoggedIn,

        /// <inheritdoc cref="WindowHostState.LoginFailed"/>
        Failed = WindowHostState.LoginFailed,
    }
}
