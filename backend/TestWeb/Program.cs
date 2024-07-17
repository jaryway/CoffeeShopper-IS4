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

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MySql");
// Add services to the container.
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

builder.Services.AddSingleton<IActionDescriptorChangeProvider>(GenericControllerActionDescriptorChangeProvider.Instance);
builder.Services.AddKeyedSingleton<IActionDescriptorChangeProvider>("DesignTime", GenericControllerActionDescriptorChangeProvider.Instance);

//builder.Services.AddScoped(typeof(BaseController<>));


builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDbContext<DynamicDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    //options.ReplaceService<IMigrator, DynamicMySqlMigrator>();
    //options.ReplaceService<IMigrationsAssembly, DynamicMigrationsAssembly>();
    //options.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
});
builder.Services.AddDbContext<DynamicDesignTimeDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    options.ReplaceService<IMigrator, DynamicMySqlMigrator>();
    options.ReplaceService<IMigrationsAssembly, DynamicMigrationsAssembly>();
    options.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OrderActionsBy(m => m.GroupName);
});

builder.Services.AddAuthorization(options =>
{
    //
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
    });
});


DynamicAssemblyBuilder.Initialize(builder.Services);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

