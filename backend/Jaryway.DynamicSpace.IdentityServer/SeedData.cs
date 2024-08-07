﻿using System.Security.Claims;
using IdentityModel;
using Jaryway.IdentityServer.EntityFramework.DbContexts;
using Jaryway.IdentityServer.EntityFramework.Mappers;
using Jaryway.IdentityServer.EntityFramework.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Jaryway.DynamicSpace.IdentityServer.Data;

namespace Jaryway.DynamicSpace.IdentityServer
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<AspNetIdentityDbContext>(
                options => options.UseSqlite(connectionString)
            );

            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AspNetIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddOperationalDbContext(
                options =>
                {
                    options.ConfigureDbContext = db =>
                        db.UseSqlite(
                            connectionString,
                            sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName)
                        );
                }
            );
            services.AddConfigurationDbContext(
                options =>
                {
                    options.ConfigureDbContext = db =>
                        db.UseSqlite(
                            connectionString,
                            sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName)
                        );
                }
            );

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            scope.ServiceProvider.GetService<PersistedGrantDbContext>()!.Database.Migrate();

            var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
            context!.Database.Migrate();

            EnsureSeedData(context);

            var ctx = scope.ServiceProvider.GetService<AspNetIdentityDbContext>();
            ctx!.Database.Migrate();
            EnsureUsers(scope);
        }

        private static void EnsureUsers(IServiceScope scope)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var angella = userMgr.FindByNameAsync("angella").Result;
            if (angella == null)
            {
                angella = new IdentityUser
                {
                    UserName = "angella",
                    Email = "angella.freeman@email.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(angella, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result =
                    userMgr.AddClaimsAsync(
                        angella,
                        new Claim[]
                        {
                            new Claim(JwtClaimTypes.Name, "Angella Freeman"),
                            new Claim(JwtClaimTypes.GivenName, "Angella"),
                            new Claim(JwtClaimTypes.FamilyName, "Freeman"),
                            new Claim(JwtClaimTypes.WebSite, "http://angellafreeman.com"),
                            new Claim("location", "somewhere")
                        }
                    ).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            foreach (var client in Config.Clients.ToList())
            {
                if (!context.Clients.Any(m => m.ClientId == client.ClientId))
                {
                    context.Clients.Add(client.ToEntity());
                }
            }

            foreach (var resource in Config.IdentityResources.ToList())
            {
                if (!context.IdentityResources.Any(m => m.Name == resource.Name))
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
            }

            context.SaveChanges();

            foreach (var apiScope in Config.ApiScopes.ToList())
            {
                if (!context.ApiScopes.Any(m => m.Name == apiScope.Name))
                {
                    context.ApiScopes.Add(apiScope.ToEntity());
                }
            }

            context.SaveChanges();

            foreach (var resource in Config.ApiResources.ToList())
            {
                if (!context.ApiResources.Any(m => m.Name == resource.Name))
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
            }

            context.SaveChanges();

        }
    }
}
