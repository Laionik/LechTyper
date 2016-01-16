using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LechTyper.Controllers
{
    public class ErrorController : Controller
    {
        // GET: /AdminError/

        public ActionResult AdminError()
        {
            return View();
        }

        // GET: /DatabaseError/
        public ActionResult DatabaseError(string errorMessage)
        {
            return View(errorMessage);
        }

    }
}
