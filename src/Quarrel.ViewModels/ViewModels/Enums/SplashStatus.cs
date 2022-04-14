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
        /// <summary>
        /// The app is loading.
        /// </summary>
        Loading = WindowHostState.Loading,

        /// <summary>
        /// Logging into an account.
        /// </summary>
        Connecting = WindowHostState.Connecting,
        
        /// <summary>
        /// Connected and playing the finishing animation.
        /// </summary>
        Connected = WindowHostState.LoggedIn,
    }
}
