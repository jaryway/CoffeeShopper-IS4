using DynamicSpace;
using DynamicSpace.Controllers;
using DynamicSpace.Design;
using DynamicSpace.Services;
using DynamicSpace.Services.Impl;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MySql");
// Add services to the container.

//builder.Services.AddSingleton<DynamicAssemblyBuilder>(new DynamicAssemblyBuilder("DynamicAssembly"));

builder.Services.AddScoped<IDynamicDesignTimeService, DynamicDesignTimeService>();

builder.Services.AddControllers(o => o.Conventions.Add(new GenericControllerRouteConvention()))
    .ConfigureApplicationPartManager(app =>
    {
        app.FeatureProviders.Add(new GenericTypeControllerFeatureProvider());
    });

builder.Services.AddSingleton<IActionDescriptorChangeProvider>(DynamicActionDescriptorChangeProvider.Instance);


builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDbContext<DynamicDesignTimeDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    options.ReplaceService<IMigrator, DynamicMySqlMigrator>();
    options.ReplaceService<IMigrationsAssembly, DynamicMigrationsAssembly>();
    options.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


DynamicAssemblyBuilder.Initialize(builder.Services);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DynamicDesignTimeDbContext>();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

