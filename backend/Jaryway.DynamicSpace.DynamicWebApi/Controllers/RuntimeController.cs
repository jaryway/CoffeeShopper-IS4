using Jaryway.DynamicSpace.DynamicWebApi.Entities;
using Jaryway.DynamicSpace.DynamicWebApi.Services;
using Jaryway.DynamicSpace.DynamicWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Jaryway.DynamicSpace.DynamicWebApi.Mappers;

namespace Jaryway.DynamicSpace.DynamicWebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RuntimeController : ControllerBase
    {
        private readonly ILogger<RuntimeController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IDynamicDesignTimeService _dynamicDesignTimeService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="dynamicDesignTimeService"></param>
        /// <param name="applicationDbContext"></param>
        public RuntimeController(ILogger<RuntimeController> logger, IDynamicDesignTimeService dynamicDesignTimeService, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _dynamicDesignTimeService = dynamicDesignTimeService;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Query")]
        public ActionResult<IEnumerable<DynamicClassModel>> Query()
        {
            var list = _dynamicDesignTimeService.GetList()
                .Select(m => m.ToModel());
            return Ok(list);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        public ActionResult<DynamicClassModel> Create([FromBody] DynamicClassModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = model.ToEntity()!;

            //var entity = new DynamicClass();
            //entity.Name = model.Name;
            //entity.TableName = model.TableName;
            //entity.EntityProperties_ = model.EntityProperties;
            //entity.JSON = model.JSON;

            _dynamicDesignTimeService.Create(entity);

            return Ok(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update/{id}")]
        public ActionResult<DynamicClassModel> Update([FromRoute] long id, [FromBody] DynamicClassModel model)
        {
            var entity = _dynamicDesignTimeService.Get(id);
            if (entity == null)
            {
                return NotFound($"对象-{id}不存在");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(model.JSON) && string.IsNullOrEmpty(model.EntityProperties))
            {
                return BadRequest("字段 JSON 和 EntityProperties 必填一个");
            }

            //var entity = new DynamicClass();
            entity.EntityProperties_ = model.EntityProperties;
            entity.JSON = model.JSON;
            _dynamicDesignTimeService.Update(entity);

            return Ok(entity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Generate")]
        public ActionResult Generate()
        {
            _dynamicDesignTimeService.Generate();
            return Ok();
        }
    }
}
