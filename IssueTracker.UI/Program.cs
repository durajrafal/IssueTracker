using IssueTracker.Application;
using IssueTracker.UI;
using IssueTracker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Initialise auth database and seed with test users
// Initialise app database
using (var scope = app.Services.CreateScope())
{
    var authInitialiser = scope.ServiceProvider.GetRequiredService<AuthDbContextInitialiser>();
    await authInitialiser.InitialiseAsync();
    await authInitialiser.SeedAsync();

    var appInitialiser = scope.ServiceProvider.GetRequiredService<AppDbContextInitialiser>();
    await appInitialiser.InitialiseAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }
