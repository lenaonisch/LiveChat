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
            ViewBag.CompanyID = StaticData.Companies[HttpContext.User.Identity.Name].ID;
            return View();
        }

        public string Log()
        {
            var s = Logger.GetHTMLAndClear();
            return s;
        }
    }
}