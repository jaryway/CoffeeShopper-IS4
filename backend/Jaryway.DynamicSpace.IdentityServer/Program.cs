using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Jaryway.DynamicSpace.IdentityServer;
using Jaryway.DynamicSpace.IdentityServer.Data;
using Microsoft.EntityFrameworkCore.Migrations;

var seed = args.Contains("/seed");
if (seed)
{
    Console.WriteLine("args", args);
    args = args.Except(new[] { "/seed" }).ToArray();
}

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;
var defaultConnectionString = builder.Configuration.GetConnectionString("Sqlite")!;
//var folder = Environment.SpecialFolder.LocalApplicationData;
//var path = Environment.GetFolderPath(folder);
//defaultConnString = defaultConnString.Replace("|DataDirectory|", path);
// System.IO.Path.Join(path, defaultConnString.Replace("Data Source=database.db",""));

if (seed)
{
    SeedData.EnsureSeedData(defaultConnectionString);
}

builder.Services.AddDbContext<AspNetIdentityDbContext>(options =>
    options.UseSqlite(defaultConnectionString,
        b => b.MigrationsAssembly(assembly)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(o => { })
    .AddEntityFrameworkStores<AspNetIdentityDbContext>();

builder.Services
    .AddIdentityServer(options =>
    {
        //options.Authentication.CookieLifetime = TimeSpan.FromDays(30);
        //options.Authentication.CookieSlidingExpiration = true;
        //options.Validation
        options.Authentication.CheckSessionCookieSameSiteMode= SameSiteMode.Unspecified;
    })
    //.AddRedirectUriValidator<PathOnlyRedirectUriValidator>()
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b =>
        {
            b.UseSqlite(defaultConnectionString, opt => opt.MigrationsAssembly(assembly));
        };

    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b =>
        {
            b.UseSqlite(defaultConnectionString, opt => opt.MigrationsAssembly(assembly));
        };
    })
    .AddDeveloperSigningCredential();

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    //options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
            //.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(myAllowSpecificOrigins);
app.UseIdentityServer();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
