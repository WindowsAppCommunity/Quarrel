// Quarrel © 2022

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// A class containing extensions for the <see cref="ServiceCollection"/> class.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Replaces the service a singleton service in the <see cref="ServiceCollection"/>.
        /// </summary>
        public static void OverrideSingleton<TService, TOldImpl, TNewImpl>(this ServiceCollection services)
            where TService : class
            where TOldImpl : class, TService
            where TNewImpl : class, TService
        {
            var oldDesc = ServiceDescriptor.Singleton<TService, TOldImpl>();
            services.Remove(oldDesc);
            services.AddSingleton<TService, TNewImpl>();
        }
    }
}
