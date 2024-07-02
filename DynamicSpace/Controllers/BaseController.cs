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
        //throw new NotImplementedException();

    }

    [HttpGet]
    [Route("{id}")]
    public T Get(long id)
    {
        //return _storage.GetById(id);
        throw new NotImplementedException();
    }

    [HttpPost()]
    [Route("{id}")]
    public void Post(long id, [FromBody] T value)
    {
        //_storage.Add(id, value);
        throw new NotImplementedException();
    }
}

