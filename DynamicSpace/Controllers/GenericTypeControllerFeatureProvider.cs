﻿using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DynamicSpace.Controllers;

public class GenericTypeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var currentAssembly = DynamicAssemblyBuilder.GetInstance(true).Assembly;

        var candidates = currentAssembly.GetExportedTypes().Where(x => x.GetCustomAttributes<GenericControllerAttribute>().Any());

        foreach (var candidate in candidates)
        {
            var typeInfo = typeof(BaseController<>).MakeGenericType(candidate).GetTypeInfo();
            feature.Controllers.Add(typeInfo);
        }
    }


}

