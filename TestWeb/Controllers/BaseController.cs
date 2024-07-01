using Microsoft.AspNetCore.Mvc;

namespace TestWeb.Controllers;

//[Route("api/[controller]")]
public class BaseController<T> : ControllerBase where T : class
{
    //private Storage<T> _storage;

    public BaseController()
    {
        //_storage = storage;
    }

    [HttpGet]
    public IEnumerable<T> Get()
    {
        //return _storage.GetAll();
        throw new NotImplementedException();

    }

    [HttpGet("{id}")]
    public T Get(Guid id)
    {
        //return _storage.GetById(id);
        throw new NotImplementedException();
    }

    [HttpPost("{id}")]
    public void Post(Guid id, [FromBody] T value)
    {
        //_storage.Add(id, value);
        throw new NotImplementedException();
    }
}
