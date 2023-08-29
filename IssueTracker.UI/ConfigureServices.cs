using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Authorization;
using IssueTracker.UI.Filters;
using IssueTracker.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text.Json.Serialization;

namespace IssueTracker.UI
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddWebUI(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add<ValidationExceptionFilter>();
            })
                .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.ConfigureHttpJsonOptions(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IEmailPreparationService, EmailPreparationService>();

            services.AddAuthentication("Cookie").AddCookie("Cookie");
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Home/AccessDenied";
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ProjectManagement", pb =>
                {
                    pb.RequireRole("Manager", "Admin");
                });
                options.AddPolicy("UserAdministration", pb =>
                {
                    pb.RequireRole("Admin");
                });
                options.AddPolicy("Development", pb =>
                {
                    pb.RequireRole("Developer", "Manager", "Admin");
                });
                options.AddPolicy("ProjectAccess", pb =>
                {
                    pb.Requirements.Add(new ProjectAccessRequirement());
                });
            });

            services.AddScoped<IAuthorizationHandler, ProjectAccessHandler>();

            return services;
        }
    }
}
