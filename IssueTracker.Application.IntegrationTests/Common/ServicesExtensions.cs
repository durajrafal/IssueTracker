using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.Application.IntegrationTests.Common
{
    public static class ServicesExtensions
    {
        public static IServiceCollection Remove<TService>(this IServiceCollection services)
        {
            var serviceDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(TService));

            services.Remove(serviceDescriptor);

            return services;
        }
    }
}
