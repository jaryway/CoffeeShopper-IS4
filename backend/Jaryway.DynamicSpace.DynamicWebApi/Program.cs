using System.Text.Json.Serialization;
using Jaryway.DynamicSpace.DynamicWebApi;
using Jaryway.DynamicSpace.DynamicWebApi.Controllers;
using Jaryway.DynamicSpace.DynamicWebApi.Design;
using Jaryway.DynamicSpace.DynamicWebApi.Services;
using Jaryway.DynamicSpace.DynamicWebApi.Services.Impl;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Sqlite");
var dataAccessAssembyName = typeof(ApplicationDbContext).Assembly.GetName().Name;
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IDynamicDesignTimeService, DynamicDesignTimeService>();

builder.Services.AddControllers(o => o.Conventions.Add(new GenericControllerModelConvention()))
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .ConfigureApplicationPartManager(app =>
    {
        app.FeatureProviders.Add(new GenericControllerFeatureProvider());
    });

builder.Services.AddAuthentication("Bearer")
    .AddIdentityServerAuthentication("Bearer", options =>
    {
        options.Authority = "https://localhost:5443";
        options.ApiName = "DynamicWebApi";
        //options.JwtValidationClockSkew = TimeSpan.FromSeconds(60);
        //options.JwtValidationClockSkew
    });

builder.Services.AddSingleton<IActionDescriptorChangeProvider>(GenericControllerActionDescriptorChangeProvider.Instance);
builder.Services.AddKeyedSingleton<IActionDescriptorChangeProvider>("DesignTime", GenericControllerActionDescriptorChangeProvider.Instance);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString, b => b.MigrationsAssembly(dataAccessAssembyName));
    //options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
builder.Services.AddDbContext<DynamicDbContext>(options =>
{
    options.UseSqlite(connectionString, b => b.MigrationsAssembly(dataAccessAssembyName));
    //options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    //options.ReplaceService<IMigrator, DynamicMySqlMigrator>();
    //options.ReplaceService<IMigrationsAssembly, DynamicMigrationsAssembly>();
    //options.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
});
builder.Services.AddDbContext<DynamicDesignTimeDbContext>(options =>
{
    options.UseSqlite(connectionString, b => b.MigrationsAssembly(dataAccessAssembyName));
    //options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    options.ReplaceService<IMigrator, DynamicMigrator>();
    options.ReplaceService<IMigrationsAssembly, DynamicMigrationsAssembly>();
    options.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OrderActionsBy(m => m.GroupName);
});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy =>
//    {
//        policy.RequireAuthenticatedUser();
//        policy.RequireRole("Admin");
//    });
//});


DynamicAssemblyBuilder.Initialize(builder.Services);

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://localhost:5443/connect/authorize"),
                TokenUrl = new Uri("https://localhost:5443/connect/token"),
                Scopes = new Dictionary<string, string> {
                    { "DynamicWebApi.all", "Access operations" },
                    //{ "DynamicWebApi.write", "Access write operations" },
                },
            }
        },

    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
    c.OrderActionsBy(m => m.GroupName);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("dynamic_web_api_swagger");
        options.OAuthClientSecret("secret");
        options.OAuthAppName("DynamicWebApi - Swagger");
        options.OAuthUsePkce();
    });
}

app.UseCors(myAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
