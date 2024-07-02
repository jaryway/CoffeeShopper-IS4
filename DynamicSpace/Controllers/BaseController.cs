using System;
using Microsoft.AspNetCore.Mvc;

namespace DynamicSpace.Controllers
{
    public class BaseController<T> : ControllerBase where T : class
    {
        public BaseController()
        {
        }
    }
}

