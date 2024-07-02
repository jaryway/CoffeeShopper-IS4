using System;
using Microsoft.AspNetCore.Mvc;

namespace DynamicSpace.Controllers;

public class BaseController<T> : ControllerBase where T : class
{
    public BaseController()
    {
    }

    [HttpGet]
    [Route("")]
    public IEnumerable<T> GetList()
    {
        //return _storage.GetAll();
        throw new NotImplementedException();

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

