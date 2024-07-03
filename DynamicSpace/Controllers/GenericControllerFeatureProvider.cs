using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace DynamicSpace.Controllers;

public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public GenericControllerFeatureProvider()
    {
    }

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var currentAssembly = DynamicAssemblyBuilder.GetInstance().Assembly;

        var candidates = currentAssembly.GetExportedTypes()
            .Where(x => x.GetCustomAttributes<GenericControllerAttribute>().Any());

        foreach (var candidate in candidates)
        {
            var typeInfo = typeof(GenericController<>).MakeGenericType(candidate).GetTypeInfo();
            feature.Controllers.Add(typeInfo);
        }
    }


}

