using DB;
using LiveChat.Hubs;
using Log;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LiveChat
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Logger.Path = Path.Combine(Server.MapPath("~"), "Log.txt");
            //Logger.Start();
            //IdentityOperations.RegisterRoles();
            var RoleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(new ApplicationDbContext()));
            if (!RoleManager.RoleExists("Operator"))
            {
                RoleManager.Create(new IdentityRole("Operator"));
            }
            if (!RoleManager.RoleExists("Owner"))
            {
                RoleManager.Create(new IdentityRole("Owner"));
            }
        }

        public static string GetCentralChatHub()
        {
            return System.Configuration.ConfigurationManager.AppSettings["CentralChatHub"].ToString(); 
        }
    }
}
