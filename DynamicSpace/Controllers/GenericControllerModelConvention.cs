using System;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;


namespace DynamicSpace.Controllers;

public class GenericControllerModelConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (controller.ControllerType.IsGenericType)
        {
            var genericType = controller.ControllerType.GenericTypeArguments[0];
            var customNameAttribute = genericType.GetCustomAttribute<GenericControllerAttribute>();
            controller.ControllerName = genericType.Name;

            if (customNameAttribute?.Route != null)
            {
                foreach (var item in controller.Selectors)
                {
                    var routeAttribute = new RouteAttribute(customNameAttribute.Route);
                    routeAttribute.Name = customNameAttribute.Name;
                    item.AttributeRouteModel = new AttributeRouteModel(routeAttribute);
                }

            }
        }
    }
}


