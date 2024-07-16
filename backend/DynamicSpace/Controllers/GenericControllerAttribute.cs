using System;
namespace DynamicSpace.Controllers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class GenericControllerAttribute : Attribute
{
    public GenericControllerAttribute(string route)
    {
        Route = route;
    }

    public string Route { get; set; }
    public string? Name { get; set; }
    //public int Order { get; set; }
}


