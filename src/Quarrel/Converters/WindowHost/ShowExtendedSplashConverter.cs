// Quarrel © 2022

using Quarrel.ViewModels.Enums;

namespace Quarrel.Converters.WindowHost
{
    public static class ShowExtendedSplashConverter
    {
        public static bool Convert(WindowHostState state)
        {
            return state switch
            {
                WindowHostState.Loading or
                WindowHostState.Connecting or 
                WindowHostState.LoginFailed => true,
                _ => false,
            };
        }
    }
}
