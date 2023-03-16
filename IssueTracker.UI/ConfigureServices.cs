using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IssueTracker.UI
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddWebUI(this IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddAuthentication("Cookie").AddCookie("Cookie");
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ProjectManagement", pb =>
                {
                    pb.RequireRole("Manager");
                });
            });

            return services;
        }
    }
}
