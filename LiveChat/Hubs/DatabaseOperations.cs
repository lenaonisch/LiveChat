using DB;
using LiveChat.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Models.ChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace LiveChat.Hubs
{
    public static class DatabaseOperations
    {

        public static bool ContainsRole(IIdentity identity, string role)
        {
            return ((ClaimsIdentity)identity).Claims
                 .Where(c => c.Type == ClaimTypes.Role)
                 .Select(c => c.Value).Contains(role);
        }

        public static void InitDatabaseData()
        {
            using (var context = new ApplicationDbContext())
            {
                var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                if (!RoleManager.RoleExists("Operator"))
                {
                    RoleManager.Create(new IdentityRole("Operator"));
                }
                if (!RoleManager.RoleExists("Owner"))
                {
                    RoleManager.Create(new IdentityRole("Owner"));
                }

                foreach(var company in context.Companies)
                {
                    StaticData.AddCompany(company);
                }

            }
        }

        public static void RegisterCompany(Company company)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Companies.Add(company);
                context.SaveChanges();
                StaticData.AddCompany(company);
            }
        }

    }
}