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
using DynamicSpace;
using DynamicSpace.Models;
using TestWeb.ViewModels;
using DynamicSpace.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using DynamicSpace.Controllers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Reflection;
using System.Text;

namespace TestWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]", Name = "DynamicClass1")]
    public class DynamicClassController : ControllerBase
    {
        private readonly ILogger<DynamicClassController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        //private readonly DynamicDesignTimeDbContext _context;
        private readonly IDynamicDesignTimeService _dynamicDesignTimeService;

        public DynamicClassController(ILogger<DynamicClassController> logger, IDynamicDesignTimeService dynamicDesignTimeService, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            //_context = context;
            _applicationDbContext = applicationDbContext;
            _dynamicDesignTimeService = dynamicDesignTimeService;
        }

        private Assembly CreateController(string name)
        {

            string code = new StringBuilder()
                .AppendLine("using System;")
                .AppendLine("using Microsoft.AspNetCore.Mvc;")
                .AppendLine("namespace TestBlocklyHtml.Controllers")
                .AppendLine("{")
                .AppendLine("[Route(\"api/[controller]\")]")
                .AppendLine("[ApiController]")
                .AppendLine(string.Format("public class {0} : ControllerBase", name))

                .AppendLine(" {")
                 .AppendLine(" [HttpGet]")
                .AppendLine("  public string Get()")
                .AppendLine("  {")
                .AppendLine(string.Format("return \"test - {0}\";", name))
                .AppendLine("  }")
                .AppendLine(" }")
                .AppendLine("}")
                .ToString();

            var codeString = SourceText.From(code);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            var references = new MetadataReference[]
             {
                 MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                 MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                 MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                 MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
                 MetadataReference.CreateFromFile(typeof(RouteAttribute).Assembly.Location),
                 MetadataReference.CreateFromFile(typeof(ApiControllerAttribute).Assembly.Location),
                 MetadataReference.CreateFromFile(typeof(ControllerBase).Assembly.Location),
                //MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                //MetadataReference.CreateFromFile(typeof(ApplicationDbContext).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System").Location),

                MetadataReference.CreateFromFile(Assembly.Load("System.Reflection").Location),
                //MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location),
                //MetadataReference.CreateFromFile(Assembly.Load("System.Linq").Location),
                //MetadataReference.CreateFromFile(Assembly.Load("System.Linq.Expressions").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                //MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore").Location),
                //MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore.Abstractions").Location),
                //MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore.Relational").Location),
                //MetadataReference.CreateFromFile(Assembly.Load("Pomelo.EntityFrameworkCore.MySql").Location),
                //MetadataReference.CreateFromFile(Assembly.Load("System.ComponentModel.Annotations").Location),
             };


            var compilation = CSharpCompilation.Create("Hello.dll")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release))
                .AddReferences(references)
                .AddSyntaxTrees([parsedSyntaxTree]);

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            //Console.WriteLine("DynamicAssemblyBuilder.Build.count" + syntaxTrees.Count());
            ms.Seek(0, SeekOrigin.Begin);

            if (!result.Success)
            {
                var errors = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);
                foreach (var diagnostic in errors)
                {
                    Console.WriteLine(diagnostic.ToString());
                    throw new Exception(diagnostic.ToString());
                }
            }


            return Assembly.Load(ms.ToArray());

            //var codeRun = CSharpCompilation.Create("Hello.dll",
            //     new[] { parsedSyntaxTree },
            //     references: references,
            //     options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
            //         optimizationLevel: OptimizationLevel.Release,
            //         assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));




            //using (var peStream = new MemoryStream())
            //{
            //    if (!codeRun.Emit(peStream).Success)
            //    {
            //        return null;
            //    }
            //    return Assembly.Load(peStream.ToArray());
            //}



        }

        [HttpGet]
        [Route("add")]
        public IEnumerable<DynamicClass> AddRuntimeController()
        {
            //string name = "andrei" + DateTime.Now.ToString("yyyyMMddHHmmss");
            ////var ass = CreateController(name);
            //var ass= DynamicAssemblyBuilder.GetInstance(true).Assembly;

            //if (ass != null)
            //{
            //    partManager.ApplicationParts.Add(new AssemblyPart(ass));
            //    // Notify change
            //    provider.HasChanged = true;
            //    provider.TokenSource.Cancel();
            //    return "api/" + name;
            //}

            return _applicationDbContext.Query<DynamicClass>().ToList();

            throw new Exception("controller not generated");
        }

        [HttpGet]
        [Route("List")]
        public ActionResult<IEnumerable<DynamicClass>> DynamicClassList()
        {

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var list = _dynamicDesignTimeService.GetList();
            //_context.AddDynamicClass(entity);

            return Ok(list);
        }

        [HttpPost]
        [Route("Addition")]
        public ActionResult<DynamicClassModel> AddDynamicClass([FromBody] DynamicClassModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = new DynamicClass();
            entity.Name = model.Name;
            entity.TableName = model.TableName;
            entity.EntityProperties_ = model.EntityProperties;

            //var builder = DynamicAssemblyBuilder.GetInstance(true);
            //builder.AddDynamicClasses(entity);
            _dynamicDesignTimeService.Create(entity);

            return Ok(entity);
        }

        [HttpPut]
        [Route("Update/{id}")]
        public ActionResult<DynamicClassModel> UpdateDynamicClass([FromRoute] long id, [FromBody] DynamicClassModel model)
        {
            var e = _dynamicDesignTimeService.Get(id);
            if (e == null)
            {
                return NotFound($"对象-{id}不存在");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = new DynamicClass();
            //entity.Name = model.Name;
            //entity.TableName = model.TableName;
            entity.EntityProperties_ = model.EntityProperties;

            //var builder = DynamicAssemblyBuilder.GetInstance(true);
            //builder.AddDynamicClasses(entity);
            _dynamicDesignTimeService.Update(entity);

            return Ok(entity);
        }

        [HttpDelete]
        [Route("Remove")]
        public ActionResult<DynamicClassModel> RemoveDynamicClass(long id)
        {

            if (id <= 0)
            {
                return BadRequest(ModelState);
            }

            var e = _dynamicDesignTimeService.Get(id);
            if (e == null)
            {
                return BadRequest("未找到记录");
            }
            //_applicationDbContext.DynamicClasses.Remove(e);
            //_applicationDbContext.SaveChanges();
            _dynamicDesignTimeService.Remove(e);
            return Ok();
        }

        //[HttpGet]
        //[Route("/Migration/List")]
        //public IEnumerable<string> GetMigrations()
        //{
        //    return _context.Database.GetMigrations();
        //}

        //// GET: DatabaseController
        //[HttpPost]
        //[Route("/Migration/Addition")]
        //public ActionResult AddMigration(string name)
        //{
        //    if (string.IsNullOrEmpty(name))
        //    {
        //        return BadRequest("参数 name 不能为空");
        //    }

        //    try
        //    {
        //        _context.AddMigration(name);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("AddMigration 报错：" + ex.Message);
        //        return BadRequest(ex.Message);
        //    }

        //    return Ok();
        //}

        //[HttpPost]
        //[Route("/Migration/Remove")]
        //public ActionResult RemoveMigration()
        //{
        //    _context.RemoveMigration(false);
        //    return Ok();
        //}

        [HttpPost]
        [Route("Generate")]
        public ActionResult Generate()
        {
            _dynamicDesignTimeService.Generate();
            return Ok();
        }
    }
}
