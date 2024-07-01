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

namespace TestWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DynamicClassController : ControllerBase
    {
        private readonly ILogger<DynamicClassController> _logger;
        //private readonly ApplicationDbContext _applicationDbContext;
        //private readonly DynamicDesignTimeDbContext _context;
        private readonly IDynamicDesignTimeService _dynamicDesignTimeService;

        public DynamicClassController(ILogger<DynamicClassController> logger, IDynamicDesignTimeService dynamicDesignTimeService)
        {
            _logger = logger;
            //_context = context;
            //_applicationDbContext = applicationDbContext;
            _dynamicDesignTimeService = dynamicDesignTimeService;
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
