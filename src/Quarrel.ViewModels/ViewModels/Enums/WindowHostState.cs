// Quarrel © 2022

namespace Quarrel.ViewModels.Enums
{
    /// <summary>
    /// An enum for the window state.
    /// </summary>
    public enum WindowHostState
    {
        /// <summary>
        /// The app is loading.
        /// </summary>
        Loading,
        
        /// <summary>
        /// The app is logging into an account.
        /// </summary>
        Connecting,

        /// <summary>
        /// The app is logged in.
        /// </summary>
        LoggedIn,

        /// <summary>
        /// The app is logged out.
        /// </summary>
        LoggedOut,
    }
}
