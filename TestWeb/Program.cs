
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TestWeb;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("mysql");
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    //var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
    //Action<SqliteDbContextOptionsBuilder> sqliteOptionsAction = o => o.MigrationsAssembly(migrationsAssembly);
    //options.UseSqlite(builder.Configuration.GetConnectionString("sqlite"), sqliteOptionsAction);

    var serverVersion = new MySqlServerVersion(Version.Parse("8.0.0"));
    var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseMySql(connectionString, serverVersion);
    //var context = new ApplicationDbContext(builder.Options);
    options.UseMySql(connectionString, serverVersion);

});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

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

