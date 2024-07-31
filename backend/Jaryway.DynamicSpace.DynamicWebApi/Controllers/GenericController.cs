using Jaryway.DynamicSpace.DynamicWebApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jaryway.DynamicSpace.DynamicWebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GenericController<T> : ControllerBase where T : DynamicClassBase
{

    private readonly DynamicDbContext _dynamicDbContext;
    private readonly ILogger<GenericController<T>> _logger;

    /// <summary>
    ///
    /// </summary>
    /// <param name="dynamicDbContext"></param>
    /// <param name="logger"></param>
    public GenericController(DynamicDbContext dynamicDbContext, ILogger<GenericController<T>> logger)
    {
        _dynamicDbContext = dynamicDbContext;
        _logger = logger;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    [HttpGet("Query")]
    //[Authorize]
    public IEnumerable<T> Query()
    {
        return _dynamicDbContext.Set<T>().ToList();
    }

    [HttpGet("{id}")]
    //[Route()]
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

    [HttpPost]
    //[Route("")]
    public ActionResult<T> Post([FromBody] T entity)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _dynamicDbContext.Set<T>().Add(entity);
        _dynamicDbContext.SaveChanges();

        return entity;
    }

    [HttpPut("{id}")]
    //[Route("{id}")]
    public ActionResult<T> Put(long id, [FromBody] T value)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = _dynamicDbContext.Set<T>().FirstOrDefault(m => m.Id == id);
        if (entity == null)
        {
            return NoContent();
        }

        var entityType = entity.GetType();

        foreach (var property in entityType.GetProperties().Where(m => m.Name != "Id"))
        {
            property.SetValue(entity, property.GetValue(value));
        }

        _dynamicDbContext.Set<T>().Update(entity);
        _dynamicDbContext.SaveChanges();

        return entity;
    }

    [HttpDelete("{id}")]
    //[Route("{id}")]
    public ActionResult<T> Delete(long id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = _dynamicDbContext.Set<T>().FirstOrDefault(m => m.Id == id);
        if (entity == null)
        {
            return NoContent();
        }

        _dynamicDbContext.Set<T>().Remove(entity);
        _dynamicDbContext.SaveChanges();

        return entity;
    }

    [HttpPost("BatchDelete")]
    //[Route("BatchDelete")]
    public ActionResult BatchDelete([FromBody] long[] ids)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


        var count = 0;
        foreach (var id in ids)
        {
            var entity = _dynamicDbContext.Set<T>().FirstOrDefault(m => m.Id == id);
            if (entity == null)
            {
                continue;
            }

            _dynamicDbContext.Set<T>().Remove(entity);
            count++;
        }

        if (count > 0)
        {
            _dynamicDbContext.SaveChanges();
        }

        return Ok();
    }
}

