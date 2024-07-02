using System;
namespace DynamicSpace.Controllers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class GeneratedControllerAttribute : Attribute
{
    public GeneratedControllerAttribute(string route)
    {
        Route = route;
    }

    public string Route { get; set; }
    public string? Name { get; set; }
}


