﻿using Lab12_Relational_DB.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab12_Relational_DB.Model
{
    public class RoleInitializer
    {
        // create a list of identity roles

        private static readonly List<IdentityRole> Roles = new List<IdentityRole>()
        {
            new IdentityRole{Name = ApplicationRoles.DistrictManager, NormalizedName = ApplicationRoles.DistrictManager.ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() },
            new IdentityRole{Name = ApplicationRoles.PropertyManager, NormalizedName = ApplicationRoles.PropertyManager.ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() },
            new IdentityRole{Name = ApplicationRoles.Agent, NormalizedName = ApplicationRoles.Agent.ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() },
        };

        public static void SeedData(IServiceProvider serviceProvider, UserManager<ApplicationUser> users, TOADD)
        {
            using (var dbContext = new AsyncInnDbContext(serviceProvider.GetRequiredService<DbContextOptions<AsyncInnDbContext>>()))
            {
                dbContext.Database.EnsureCreated();
                AddRoles(dbContext);
                //SeedUsers(users, _config);
            }
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager, IConfiguration _config)
        {
            if(userManager.FindByEmailAsync(_config["AdminEmail"]).Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = _config["Admin"]
            }

        }

        private static void AddRoles(AsyncInnDbContext context)
        {
            if (context.Roles.Any()) return;

            foreach (var role in Roles)
            {
                context.Roles.Add(role);
                context.SaveChanges();
            }
        }

    }
}
