using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;


namespace DynamicSpace.Controllers;

public class GenericControllerRouteConvention : IControllerModelConvention
{
    public GenericControllerRouteConvention()
    {
    }

    public void Apply(ControllerModel controller)
    {
        if (controller.ControllerType.IsGenericType)
        {
            var genericType = controller.ControllerType.GenericTypeArguments[0];
            var customNameAttribute = genericType.GetCustomAttribute<GeneratedControllerAttribute>();

            if (customNameAttribute?.Route != null)
            {
                var routeAttribute = new RouteAttribute(customNameAttribute.Route);
                routeAttribute.Name = customNameAttribute.Name;
                controller.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel(routeAttribute),
                });
            }
            else
            {
                controller.ControllerName = genericType.Name;
            }
        }
    }
}


