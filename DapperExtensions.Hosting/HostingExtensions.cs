using Microsoft.Extensions.DependencyInjection;
using SharpVNC.Client.DataAccess;
using System;

namespace DapperExtensions.Hosting
{
    /// <summary>
    /// Extension methods for hosting DapperExtensions within ASP .NET Core.
    /// </summary>
    public static class HostingExtensions
    {
        /// <summary>
        /// Adds the necessary services for DapperExtensions to the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection to configure as an instance of <see cref="IServiceCollection"/>.</param>
        /// <returns>An instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddDapperExtensions(this IServiceCollection serviceCollection)
        {
            if (serviceCollection is null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.AddSingleton<RepositoryNamingProvider>
        }
    }
}