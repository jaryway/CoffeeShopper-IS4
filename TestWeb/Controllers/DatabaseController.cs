using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Internal;
using Microsoft.EntityFrameworkCore.Sqlite.Design.Internal;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Sqlite.Scaffolding.Internal;
//using MySql.EntityFrameworkCore;
//using MySql.EntityFrameworkCore.Migrations;
//using MySql.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using Pomelo.EntityFrameworkCore.MySql.Design.Internal;
using System.Xml.Linq;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace TestWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatabaseController : ControllerBase
    {

        private readonly ILogger<DatabaseController> _logger;

        private readonly ApplicationDbContext _context;

        private readonly IServiceProvider _serviceProvider;

        private readonly string rootNamespace = "TestWeb";
        private readonly string projectDir = Directory.GetCurrentDirectory();
        private readonly string outputDir = "./Migrations/";

        public DatabaseController(ILogger<DatabaseController> logger, ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _context = context;
            _serviceProvider = serviceProvider;
        }

        private IMigrationsScaffolder GetMigrationsScaffolder(ApplicationDbContext context)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddEntityFrameworkDesignTimeServices();
            serviceCollection.AddDbContextDesignTimeServices(context);
            new MySqlDesignTimeServices().ConfigureDesignTimeServices(serviceCollection);

            //serviceCollection.AddScoped<AnnotationCodeGeneratorDependencies>();
            //serviceCollection.AddScoped<TypeMappingSourceDependencies>();
            //serviceCollection.AddScoped<ValueConverterSelectorDependencies>();
            //serviceCollection.AddScoped<RelationalTypeMappingSourceDependencies>();
            //serviceCollection.AddSingleton<IValueConverterSelector, ValueConverterSelector>();
            //serviceCollection.AddSingleton<ITypeMappingSource, MySqlTypeMappingSource>();
            //serviceCollection.AddSingleton<IAnnotationCodeGenerator, MySqlAnnotationCodeGenerator>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var migrationsScaffolder = serviceProvider.GetRequiredService<IMigrationsScaffolder>();

            return migrationsScaffolder;
        }

        [HttpGet]
        [Route("/Migration/List")]
        public IEnumerable<string> GetMigrations()
        {
            return _context.Database.GetMigrations();
        }


        // GET: DatabaseController
        [HttpPost]
        [Route("/Migration/Addition")]
        public ActionResult AddMigration(string name)
        {

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("参数 name 不能为空");
            }

            if (!_context.Database.HasPendingModelChanges())
            {
                return NoContent();
            }

            var migrationName = (name ?? "") + DateTime.Now.Ticks.ToString();
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var migrationsScaffolder = GetMigrationsScaffolder(db);
                var migration = migrationsScaffolder.ScaffoldMigration(migrationName, rootNamespace);
                var files = migrationsScaffolder.Save(projectDir, migration, outputDir);
            }

            return Ok();
        }

        [HttpPost]
        [Route("/Migration/Remove")]
        public ActionResult RemoveMigration()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                //_serviceProvider.GetService
                //var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                var serverVersion = new MySqlServerVersion(Version.Parse("8.4.0"));
                var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseMySql("server=localhost;uid=root;pwd=123456;database=test", serverVersion);
                var context = new ApplicationDbContext(builder.Options);
                //var migrationsScaffolder = GetMigrationsScaffolder(context);
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddEntityFrameworkDesignTimeServices();
                serviceCollection.AddDbContextDesignTimeServices(context);
                var designTimeServices = new MySqlDesignTimeServices();
                designTimeServices.ConfigureDesignTimeServices(serviceCollection);


                //serviceCollection.AddScoped<AnnotationCodeGeneratorDependencies>();
                //serviceCollection.AddScoped<TypeMappingSourceDependencies>();
                //serviceCollection.AddScoped<ValueConverterSelectorDependencies>();
                //serviceCollection.AddScoped<RelationalTypeMappingSourceDependencies>();
                //serviceCollection.AddSingleton<IValueConverterSelector, ValueConverterSelector>();
                //serviceCollection.AddSingleton<ITypeMappingSource, MySqlTypeMappingSource>();
                //serviceCollection.AddSingleton<IAnnotationCodeGenerator, MySqlAnnotationCodeGenerator>();

                var serviceProvider = serviceCollection.BuildServiceProvider();
                var migrationsScaffolder = serviceProvider.GetRequiredService<IMigrationsScaffolder>();
                var fiels = migrationsScaffolder.RemoveMigration(projectDir, null, true, null);

                designTimeServices = null;
            }

            return Ok();
        }

        [HttpPost]
        [Route("Update")]
        public ActionResult Update()
        {
            _context.Database.Migrate();
            return Ok();
        }


    }
}
