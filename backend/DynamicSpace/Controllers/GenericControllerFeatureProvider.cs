using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing.Matching;

namespace DynamicSpace.Controllers;

public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public GenericControllerFeatureProvider()
    {
    }

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        AddControllers(DynamicAssemblyBuilder.GetInstance().Assembly, feature);
        AddControllers(typeof(GenericController<>).Assembly, feature);
    }
    private void AddControllers(Assembly assembly, ControllerFeature feature)
    {
        var candidates = assembly.GetExportedTypes()
            .Where(x => x.GetCustomAttributes<GenericControllerAttribute>().Any());

        foreach (var candidate in candidates)
        {
            var typeInfo = typeof(GenericController<>).MakeGenericType(candidate).GetTypeInfo();
            feature.Controllers.Add(typeInfo);
        }
    }


}

