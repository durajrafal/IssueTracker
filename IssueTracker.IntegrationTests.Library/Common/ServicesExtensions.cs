using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace IssueTracker.IntegrationTests.Library.Common
{
    internal static class ServicesExtensions
    {
        internal static IServiceCollection Remove<TService>(this IServiceCollection services)
        {
            var serviceDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(TService));

            services.Remove(serviceDescriptor);

            return services;
        }

        internal static void Mock<TService>(this IServiceCollection services, Action<Mock<TService>> customize) where TService : class
        {
            var serviceType = typeof(TService);
            if (services.FirstOrDefault(x => x.ServiceType == serviceType) is { } existingServiceDescriptor)
            {
                services.Replace(new ServiceDescriptor(serviceType, _ =>
                {
                    var mock = new Mock<TService>();
                    customize(mock);
                    return mock.Object;
                }, existingServiceDescriptor.Lifetime));
                return;
            }

            throw new InvalidOperationException($"'{serviceType}' was not registered in DI Container");
        }
    }
}
