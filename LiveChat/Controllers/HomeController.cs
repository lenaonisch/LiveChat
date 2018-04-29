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

        public ActionResult Chat()
        {
            return View();
        }

        [Authorize(Roles="Operator")]
        public ActionResult OperatorChat()
        {
            return View();
        }
        [Authorize(Roles = "Owner")]
        public ActionResult PrivateOffice()
        {
            return View();
        }
    }
}