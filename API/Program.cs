using System.Reflection;
using API;
using API.Services;
using DataAccess.Data;
//using Jaryway.Net.LowCode.User;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var dataAccessAssemby = Assembly.Load("DataAccess");
var dataAccessAssembyName = dataAccessAssemby.GetName().Name;
var lowCodeUserAssemby = Assembly.Load("Jaryway.Net.LowCode.User");
var lowCodeUserAssembyName = lowCodeUserAssemby.GetName().Name;

var assemblyName = typeof(Program).Assembly.GetName().Name;
var defaultConnString = builder.Configuration.GetConnectionString("DefaultConnection");
var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
defaultConnString = defaultConnString.Replace("|DataDirectory|", path);
// System.IO.Path.Join(path, );
//builder.AddApplicationPart()

//var dataAccessAssemby = Assembly.Load("Jaryway.Net.LowCode.User");

builder.Services.AddControllers()
    //.AddApplicationPart(dataAccessAssemby)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

builder.Services.AddAuthentication("Bearer")
    .AddIdentityServerAuthentication("Bearer", options =>
    {
        options.Authority = "https://localhost:5443";
        options.ApiName = "CoffeeAPI";
        options.JwtValidationClockSkew = TimeSpan.FromSeconds(10);
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(defaultConnString, b => b.MigrationsAssembly(dataAccessAssembyName)));
//builder.Services.AddDbContext<LowCodeUserDbContext>(options =>
//    options.UseSqlite(defaultConnString, b => b.MigrationsAssembly(lowCodeUserAssembyName)));

builder.Services.AddScoped<ICoffeeShopService, CoffeeShopService>();
builder.Services.AddScoped(provider => provider);




var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    //options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
            //.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


var app = builder.Build();
//var mgr = new MigrationManager(app.Services.GetRequiredService<IServiceProvider>());
//mgr.AddMigration("");



app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
