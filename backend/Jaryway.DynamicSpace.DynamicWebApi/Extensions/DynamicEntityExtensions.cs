using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Jaryway.DynamicSpace.DynamicWebApi.Models;
using Jaryway.DynamicSpace.DynamicWebApi.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using Jaryway.DynamicSpace.DynamicWebApi.Controllers;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.OpenApi.Extensions;

namespace Jaryway.DynamicSpace.DynamicWebApi
{
    /// <summary>
    /// 
    /// </summary>
    public static class DynamicEntityExtensions
    {
        private static string GetProperties(DynamicClass entity)
        {
            if (string.IsNullOrEmpty(entity.JSON))
            {
                return entity.EntityProperties_;
            }

            var fields = JsonSerializer.Deserialize<IList<DynamicClassFieldDefinition>>(entity.JSON);

            if (fields == null)
            {
                return "";
            }

            var fields_ = fields.Select(field =>
            {
                var dataType = field.DataType.GetDisplayName();
                var defaultValue = field.DefaultValue;
                var arr = new string[] { "public", dataType, field.Name, "{ get; set; }" }.ToList();

                if (!string.IsNullOrEmpty(defaultValue))
                {
                    arr.Add("=");
                    arr.Add(field.DataType == DynamicClassFieldDataType.TEXT ? $"\"{defaultValue}\"" : defaultValue);
                }

                return string.Join(" ", arr);
            });

            return string.Join("\r\n", fields_);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="designTime"></param>
        /// <returns></returns>
        public static string GenerateCode(this DynamicClass entity, bool designTime = false)
        {
            var genericTypeControllerType = typeof(GenericControllerAttribute);
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
    {(designTime ? GetProperties(entity) : entity.EntityProperties)}
}}";
            return code;
        }

    }
}
