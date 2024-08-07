﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Jaryway.DynamicSpace.WebApi
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //var requiredScopes = context.MethodInfo
            //.GetCustomAttributes(true)
            //.OfType<AuthorizeAttribute>()
            //.Select(attr => attr.Policy)
            //.Distinct();

            //if (requiredScopes.Any())
            //{
            //    operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            //    operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            //    var oAuthScheme = new OpenApiSecurityScheme
            //    {
            //        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            //    };

            //    operation.Security = new List<OpenApiSecurityRequirement>
            //    {
            //        new OpenApiSecurityRequirement
            //        {
            //            [ oAuthScheme ] = requiredScopes.ToList()
            //        }
            //    };
            //}

            var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecurityScheme {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "oauth2"
                                }
                            }
                        ] = new[] {"api1"}
                    }
                };

            }
        }
    }
}
