using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LechTyper.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "LechTyper";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Title = "Kontakt";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "O mnie";
            return View();
        }
    }
}
