using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Jaryway.DynamicSpace.DynamicWebApi.Attributes;
using Jaryway.DynamicSpace.DynamicWebApi.Controllers;
using Jaryway.DynamicSpace.DynamicWebApi.Entities;
using Microsoft.OpenApi.Extensions;
using System.ComponentModel.DataAnnotations;
using System;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Jaryway.DynamicSpace.DynamicWebApi
{


    /// <summary>
    /// 
    /// </summary>
    public static class DynamicEntityExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetDesignTimeProperties(this DynamicClass entity)
        {
            if (string.IsNullOrEmpty(entity.JSON))
            {
                return entity.EntityProperties_;
            }

            //var str = $"[{{\"name\":\"Name\",\"dataType\":1}}]";

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            var fields = JsonSerializer.Deserialize<IList<DynamicClassFieldDefinition>>(entity.JSON, options);

            if (fields == null)
            {
                return "";
            }

            var fields_ = fields.Select(field =>
            {
                //var dataType1 = field.DataType.GetDisplayName();
                var attributeOfType = field.DataType.GetAttributeOfType<DisplayAttribute>();
                var dataType = attributeOfType.Name ?? field.DataType.ToString();

                // 对象引用
                if (field.DataType == DynamicClassFieldDataType.REFERENCE)
                {
                    dataType = field.Reference;
                }

                var defaultValue = field.DefaultValue;
                var arr = new string[] { "public", dataType, field.Name, "{ get; set; }" }.ToList();

                if (!string.IsNullOrEmpty(defaultValue))
                {
                    arr.Add("=");
                    arr.Add(field.DataType == DynamicClassFieldDataType.TEXT ? $"\"{defaultValue}\";" : $"{defaultValue};");
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
    {(designTime ? GetDesignTimeProperties(entity) : entity.EntityProperties)}
}}";
            return code;
        }

    }
}
