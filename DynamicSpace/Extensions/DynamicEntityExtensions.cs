using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DynamicSpace.Models;
using DynamicSpace.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using DynamicSpace.Controllers;

namespace DynamicSpace
{
    public static class DynamicEntityExtensions
    {
        public static string GenerateCode(this DynamicClass entity, bool designTime = false)
        {
            var genericTypeControllerType = typeof(GenericTypeControllerAttribute);
            var genericTypeController = genericTypeControllerType.Name.Replace("Attribute", "");
            var dynamicClassBaseType = typeof(DynamicClassBase);

            var code = $@"
using System;
using System.Collections.Generic;
using {dynamicClassBaseType.Namespace};
using {typeof(EntityIdAttribute).Namespace};
using {typeof(TableAttribute).Namespace};
using {genericTypeControllerType.Namespace};
[EntityId({entity.Id})]
[Table(""Dynamic_{entity.TableName}"")]
[{genericTypeController}(""api/{entity.Name}"")]
public class {entity.Name} : {dynamicClassBaseType.Name}{{
    {(designTime ? entity.EntityProperties_ : entity.EntityProperties)}
}}";
            return code;
        }

    }
}
