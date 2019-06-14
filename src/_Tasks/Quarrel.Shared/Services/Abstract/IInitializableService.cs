// Special thanks to Sergio Pedri for the basis of this design

using System.Threading.Tasks;

namespace Quarrel.Services.Abstract
{
    /// <summary>
    /// A base <see langword="interface"/> for a service that requires initialization before being used
    /// </summary>
    internal interface IInitializableService
    {
        /// <summary>
        /// Initializes the services and performs all the necessary preliminary operations
        /// </summary>
        Task InitializeAsync();
    }
}
