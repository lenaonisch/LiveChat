using Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveChat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Chat()
        {
            ViewBag.UserName = User.Identity.Name;
            return View();
        }

        [Authorize(Roles="Operator")]
        public ActionResult OperatorChat()
        {
            ViewBag.UserName = User.Identity.Name;
            return View();
        }

        [Authorize(Roles = "Owner")]
        public ActionResult PrivateOffice()
        {
            ViewBag.HtmlCode = @"<!DOCTYPE html>" +
@"<html>" +
@"<head>" +
@"    <meta charset='utf-8' />" +
@"    <meta name='viewport' content='width=device-width, initial-scale=1.0'>" +
@"</head>" +
@"<body>" +
@"	<script>" +
@"		window.CentralChatHub = '" + MvcApplication.GetCentralChatHub() + "';" +
@"		window.CompanyID='" + StaticData.Companies[HttpContext.User.Identity.Name].ID + "';" +
@"		window.ChatContainerName='ChatContainer';" +
@"	</script>" +
@"	<div class='ChatContainer'></div>" +
@"	<link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css'>" +
@"	<script src='https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js'></script>" +
@"	<script src='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js'></script>" +
@"	<script src='" + MvcApplication.GetCentralChatHub() + "/Scripts/jquery.signalR-2.2.3.min.js'></script>" +
@"	<script src='" + MvcApplication.GetCentralChatHub() + "/signalr/hubs'></script>" +
@"	<script src='" + MvcApplication.GetCentralChatHub() + "/Scripts/LiveChat.js'></script>" +
@"</body>" +
@"</html>";
            return View();
        }

        public string Log()
        {
            var s = Logger.GetHTMLAndClear();
            return s;
        }
    }
}