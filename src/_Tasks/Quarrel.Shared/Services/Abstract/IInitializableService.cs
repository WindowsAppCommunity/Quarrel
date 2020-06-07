// Special thanks to Sergio Pedri for this design from Legere
// GitHub profile: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using System.Threading.Tasks;

namespace Quarrel.Services.Abstract
{
    /// <summary>
    /// A base <see langword="interface"/> for a service that requires initialization before being used.
    /// </summary>
    internal interface IInitializableService
    {
        /// <summary>
        /// Initializes the services and performs all the necessary preliminary operations.
        /// </summary>
        /// <returns>The service.</returns>
        Task InitializeAsync();
    }
}
