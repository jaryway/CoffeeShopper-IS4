using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Jaryway.DynamicSpace.IdentityServer;
using Jaryway.DynamicSpace.IdentityServer.Data;
using Jaryway.IdentityServer.Extensions;
using Jaryway.IdentityServer;

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

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AspNetIdentityDbContext>();

//builder.Services.AddAuthorization(options=>options.AddPolicy());

builder.Services.AddSameSiteCookiePolicy();

builder.Services
    .AddIdentityServer()
    .AddRedirectUriValidator<PathOnlyRedirectUriValidator>()
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

//builder.Services.AddAuthentication(IdentityServerConstants.DefaultCookieAuthenticationScheme)
//    .AddCookie(IdentityServerConstants.DefaultCookieAuthenticationScheme, (options) =>
//    {
//        options.Cookie.SameSite = SameSiteMode.Lax;
//        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
//    });


//builder.Services
//    .AddAuthentication(options =>
//    {
//        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

//        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//        options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//        options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    })
//    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//    {
//        options.Cookie.Name = IdentityServerConstants.DefaultCheckSessionCookieName;
//        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//        options.Cookie.SameSite = SameSiteMode.None;
//    });

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
app.UseCookiePolicy();
app.UseIdentityServer();

app.UseAuthorization();


app.MapControllerRoute
(
    name: "home",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


app.Run();
