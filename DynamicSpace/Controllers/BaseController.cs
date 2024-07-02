using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using DynamicSpace.Attributes;
using Microsoft.Extensions.Logging;
using DynamicSpace.Models;

namespace DynamicSpace.Controllers;

public class BaseController<T> : ControllerBase where T : DynamicClassBase
{

    private readonly DynamicDbContext _dynamicDbContext;
    private readonly ILogger<BaseController<T>> _logger;

    public BaseController(DynamicDbContext dynamicDbContext, ILogger<BaseController<T>> logger)
    {
        _dynamicDbContext = dynamicDbContext;
        _logger = logger;
    }

    [HttpGet]
    [Route("")]
    public IEnumerable<T> GetList()
    {
        var typeInfo = typeof(T)!;
        var classId = typeInfo.GetCustomAttribute<EntityIdAttribute>()!.EntityId;
        return _dynamicDbContext.Query<T>().ToList();
    }

    [HttpGet]
    [Route("{id}")]
    public ActionResult<T?> Get(long id)
    {
        try
        {
            return _dynamicDbContext.Set<T>().FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get by id");
            return BadRequest("ex:" + ex.Message);
        }
    }

    [HttpPost()]
    [Route("{id}")]
    public ActionResult<T> Post(long id, [FromBody] T value)
    {
        //_storage.Add(id, value);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = _dynamicDbContext.Set<T>().FirstOrDefault();
        if (entity == null)
        {
            return BadRequest($"对象 {id} 未找到");
        }
        

        _dynamicDbContext.Set<T>().Update(value);

        return value;

        //throw new NotImplementedException();
    }
}

