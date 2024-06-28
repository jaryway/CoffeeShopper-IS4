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

namespace TestWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatabaseController : ControllerBase
    {

        private readonly ILogger<DatabaseController> _logger;

        private readonly DynamicDbContext _context;

        public DatabaseController(ILogger<DatabaseController> logger, DynamicDbContext context, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [Route("/DynamicEntity/Add")]
        public ActionResult<DynamicEntityModel> AddDynamicEntity([FromBody] DynamicEntityModel model)
        {

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var entity = new DynamicEntity();
            entity.Name = model.Name;
            entity.TableName = model.TableName;
            entity.EntityProperties = model.EntityProperties;

           _context.AddDynamicEntity(entity);

            return Ok(entity);
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

            try
            {
                _context.AddMigration(name);
            }
            catch (Exception ex)
            {
                _logger.LogError("AddMigration 报错：" + ex.Message);
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost]
        [Route("/Migration/Remove")]
        public ActionResult RemoveMigration()
        {
            _context.RemoveMigration(true);
            return Ok();
        }

        [HttpPost]
        [Route("Update")]
        public ActionResult Update()
        {
            _context.UpdateDatabase();
            return Ok();
        }


    }
}
