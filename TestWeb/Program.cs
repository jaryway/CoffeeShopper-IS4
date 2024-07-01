using DynamicSpace;
using DynamicSpace.Design;
using DynamicSpace.Services;
using DynamicSpace.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MySql");
// Add services to the container.

//builder.Services.AddSingleton<DynamicAssemblyBuilder>(new DynamicAssemblyBuilder("DynamicAssembly"));

builder.Services.AddScoped<IDynamicDesignTimeService, DynamicDesignTimeService>();

builder.Services.AddControllers();

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

//var s1 = DynamicAssemblyBuilder.GetInstance();
//var s2 = DynamicAssemblyBuilder.GetInstance();
//var s3 = DynamicAssemblyBuilder.GetInstance(true);

//s2.AddDynamicEntities(new DynamicSpace.Models.DynamicClass { Id = 1, Name = "1" });


var app = builder.Build();
DynamicAssemblyBuilder.Initialize(app.Services);
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DynamicDesignTimeDbContext>();
    //var h1 = context.Database.HasPendingModelChanges();
    //var h2 = context.Database.HasPendingModelChanges();
    //var migrations = context.MigrationEntries.ToList() ?? [];
    //var entities = context.DynamicEntities.ToList() ?? [];

    //var migrationsCode = migrations.Select(p => new KeyValuePair<string, string>(p.MigrationId, p.Code));
    //var entitiesCode = entities.Select(p => new KeyValuePair<string, string>(string.Join(".", "DynamicAssembly", p.Name), p.GenerateCode()));

    //DynamicAssemblyBuilder.Initialize(migrationsCode.Concat(entitiesCode));
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

