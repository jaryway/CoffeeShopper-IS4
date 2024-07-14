
using DynamicSpace;
using DynamicSpace.Models;
using DynamicSpace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestWeb.ViewModels;

namespace TestWeb.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DynamicClassController : ControllerBase
    {
        private readonly ILogger<DynamicClassController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IDynamicDesignTimeService _dynamicDesignTimeService;

        public DynamicClassController(ILogger<DynamicClassController> logger, IDynamicDesignTimeService dynamicDesignTimeService, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _dynamicDesignTimeService = dynamicDesignTimeService;
        }

        [HttpGet]
        [Route("Query")]
        public ActionResult<IEnumerable<DynamicClass>> Query()
        {
            var list = _dynamicDesignTimeService.GetList();
            return Ok(list);
        }

        [HttpPost]
        [Route("Create")]
        public ActionResult<DynamicClassModel> Create([FromBody] DynamicClassModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = new DynamicClass();
            entity.Name = model.Name;
            entity.TableName = model.TableName;
            entity.EntityProperties_ = model.EntityProperties;

            _dynamicDesignTimeService.Create(entity);

            return Ok(entity);
        }

        [HttpPut]
        [Route("Update/{id}")]
        public ActionResult<DynamicClassModel> Update([FromRoute] long id, [FromBody] DynamicClassModel model)
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
            entity.EntityProperties_ = model.EntityProperties;
            _dynamicDesignTimeService.Update(entity);

            return Ok(entity);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public ActionResult<DynamicClassModel> Remove(long id)
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

            _dynamicDesignTimeService.Remove(e);
            return Ok();
        }

        [HttpPost]
        [Route("Generate")]
        public ActionResult Generate()
        {
            _dynamicDesignTimeService.Generate();
            return Ok();
        }
    }
}
