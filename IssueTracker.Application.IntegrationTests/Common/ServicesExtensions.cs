using IssueTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
