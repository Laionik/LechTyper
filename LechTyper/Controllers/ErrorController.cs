using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LechTyper.Controllers
{
    public class ErrorController : Controller
    {

        /// <summary>
        /// Brak uprawnień administratora
        /// </summary>
        /// <returns>Widok</returns>
        public ActionResult AdminError()
        {
            return View();
        }

        /// <summary>
        /// Błąd bazy danych
        /// </summary>
        /// <returns>Widok</returns>
        public ActionResult DatabaseError(string errorMessage)
        {
            return View(errorMessage);
        }
        /// <summary>
        /// Błąd ogólny
        /// </summary>
        /// <returns>Widok</returns>
        public ActionResult Error(string errorMessage)
        {
            return View(errorMessage);
        }

    }
}
