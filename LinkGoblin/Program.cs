using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LinkGoblin.Components;
using LinkGoblin.Components.Account;
using LinkGoblin.Data;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

var auth = builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    });
if (bool.Parse(builder.Configuration["OpenIdConnect:Enabled"] ?? "false"))
{
    auth.AddOpenIdConnect(config =>
    {
        config.RequireHttpsMetadata = true;
        config.MetadataAddress = builder.Configuration["OpenIdConnect:MetadataAddress"];
        config.ClientId = builder.Configuration["OpenIdConnect:ClientId"];
        config.ClientSecret = builder.Configuration["OpenIdConnect:ClientSecret"];
        config.NonceCookie.SameSite = SameSiteMode.Strict;
        config.CorrelationCookie.SameSite = SameSiteMode.Strict;
    });
}
auth.AddIdentityCookies();

#region Datasource
try
{
    var provider = builder.Configuration.GetSection("App")["DatabaseType"]?.ToUpper() ?? throw new InvalidOperationException("Database provider not found.");
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContextPool<ApplicationDbContext>(options => _ = provider switch
    {
        "NPGSQL" => options.UseNpgsql(connectionString, x => x.MigrationsAssembly("LinkGoblin.Migrations.PostgreSql")),
        "SQLITE" => options.UseSqlite(connectionString, x => x.MigrationsAssembly("LinkGoblin.Migrations.Sqlite")),
        _ => throw new InvalidOperationException("Database provider not found.")
    });
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
#endregion
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddRadzenComponents();

builder.Services.ConfigureApplicationCookie(config =>
{
    config.Cookie.SameSite = SameSiteMode.Strict;
    config.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    config.Cookie.HttpOnly = true;
});

var app = builder.Build();

#region Security Headers
var policyCollection = new HeaderPolicyCollection()
    .AddFrameOptionsDeny()
    .AddXssProtectionBlock()
    .AddContentTypeOptionsNoSniff()
    .AddReferrerPolicySameOrigin()
    .RemoveServerHeader()
    .AddContentSecurityPolicyReportOnly(cspBuilder =>
    {
        cspBuilder.AddUpgradeInsecureRequests();
        cspBuilder.AddBlockAllMixedContent();
        cspBuilder.AddFontSrc().Self();
        cspBuilder.AddDefaultSrc().Self();
        cspBuilder.AddConnectSrc().Self();
        cspBuilder.AddObjectSrc().Self();
        cspBuilder.AddFormAction().Self();
        cspBuilder.AddImgSrc().Self();
        cspBuilder.AddScriptSrc().Self().UnsafeInline().UnsafeEval().ReportSample();
        cspBuilder.AddStyleSrc().Self().StrictDynamic();
        cspBuilder.AddMediaSrc().Self();
        cspBuilder.AddFrameAncestors().None();
        cspBuilder.AddBaseUri().Self();
        cspBuilder.AddFrameSrc().Self();
    });

app.UseSecurityHeaders(policyCollection);
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Migrate the database on startup
await using var scope = app.Services.CreateAsyncScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await dbContext.Database.MigrateAsync();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
