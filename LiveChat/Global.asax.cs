using DB;
using LiveChat.Hubs;
using Log;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            //Database.SetInitializer(new ApplicationDBDropCreate());

            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(300);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DatabaseOperations.InitDatabaseData();
            
        }

        public static string GetCentralChatHub()
        {
            return System.Configuration.ConfigurationManager.AppSettings["CentralChatHub"].ToString(); 
        }
    }
}
