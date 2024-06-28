using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DynamicSpace.Models;
using DynamicSpace.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicSpace
{
    public static class DynamicEntityExtensions
    {
        public static string GenerateCode(this DynamicEntity entity)
        {

            var code = $@"
using System;
using System.Collections.Generic;
using {typeof(EntityBase).Namespace};
using {typeof(EntityIdAttribute).Namespace};
using {typeof(TableAttribute).Namespace};
[EntityId({entity.Id})]
[Table(""Dynamic_{entity.TableName}"")]
public class {entity.Name} : EntityBase{{
    {entity.EntityProperties}
}}";
            return code;
        }

    }
}
