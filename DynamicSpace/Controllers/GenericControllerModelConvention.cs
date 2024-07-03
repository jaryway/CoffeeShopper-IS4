using System;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;


namespace DynamicSpace.Controllers;

public class GenericControllerModelConvention : IControllerModelConvention
{
    public GenericControllerModelConvention()
    {
    }

    public void Apply(ControllerModel controller)
    {
        if (controller.ControllerType.IsGenericType)
        {
            var genericType = controller.ControllerType.GenericTypeArguments[0];
            var customNameAttribute = genericType.GetCustomAttribute<GenericControllerAttribute>();
            controller.ControllerName = genericType.Name;
            //controller.Filters.Add(new AuthorizeAttribute());

            if (customNameAttribute?.Route != null)
            {
                var routeAttribute = new RouteAttribute(customNameAttribute.Route);
                routeAttribute.Name = customNameAttribute.Name;
                //routeAttribute.Order = customNameAttribute.Order;

                controller.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel(routeAttribute),
                });
            }
        }
    }
}


