using IssueTracker.Application.Common.Interfaces;
using IssueTracker.UI.Authorization;
using IssueTracker.UI.Filters;
using IssueTracker.UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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
                options.Filters.Add<UnauthorizedAccessExceptionFilter>();
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
                options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden, options.Events.OnRedirectToAccessDenied);
                options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized, options.Events.OnRedirectToLogin);
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
        private static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode, 
            Func<RedirectContext<CookieAuthenticationOptions>, Task> existingRedirector) =>
        context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = (int)statusCode;
                return Task.CompletedTask;
            }
            return existingRedirector(context);
        };
    }
}
