using System.Reflection;
using Jaryway.DynamicSpace.WebApi;
using Jaryway.DynamicSpace.WebApi.Services;
using Jaryway.DynamicSpace.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var dataAccessAssembyName = typeof(ApplicationDbContext).Assembly.GetName().Name;

var defaultConnString = builder.Configuration.GetConnectionString("DefaultConnection");
var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
defaultConnString = defaultConnString.Replace("|DataDirectory|", path);

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
        //options.JwtValidationClockSkew = TimeSpan.FromSeconds(60);
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(defaultConnString, b => b.MigrationsAssembly(dataAccessAssembyName)));

builder.Services.AddScoped<ICoffeeShopService, CoffeeShopService>();

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
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
                    { "CoffeeAPI.read", "Access read operations" },
                    { "CoffeeAPI.write", "Access write operations" },
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
        options.OAuthClientId("api_swagger");
        options.OAuthClientSecret("secret");
        options.OAuthAppName("CoffeeAPI - Swagger");
        options.OAuthUsePkce();
    });
}

app.UseCors(myAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
