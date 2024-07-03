using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DynamicSpace.Controllers;

public class GenericTypeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public GenericTypeControllerFeatureProvider()
    {
    }

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var currentAssembly = DynamicAssemblyBuilder.GetInstance().Assembly;

        var candidates = currentAssembly.GetExportedTypes()
            .Where(x => x.GetCustomAttributes<GenericTypeControllerAttribute>().Any());

        foreach (var candidate in candidates)
        {
            var typeInfo = typeof(BaseController<>).MakeGenericType(candidate).GetTypeInfo();
            feature.Controllers.Add(typeInfo);
        }
    }


}

