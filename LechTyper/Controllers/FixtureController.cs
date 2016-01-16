using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LechTyper.Models;

namespace LechTyper.Controllers
{
    public class FixtureController : Controller
    {
        private FixtureContext db = new FixtureContext();

        //
        // GET: /Fixture/

        public ActionResult Index()
        {
            return View(db.Fixtures.ToList());
        }

        //
        // GET: /Fixture/Details/5

        public ActionResult Details(int id = 0)
        {
            Fixture fixture = db.Fixtures.Find(id);
            if (fixture == null)
            {
                return HttpNotFound();
            }
            return View(fixture);
        }

        //
        // GET: /Fixture/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Fixture/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Fixture fixture)
        {
            if (ModelState.IsValid)
            {
                db.Fixtures.Add(fixture);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fixture);
        }

        //
        // GET: /Fixture/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Fixture fixture = db.Fixtures.Find(id);
            if (fixture == null)
            {
                return HttpNotFound();
            }
            return View(fixture);
        }

        //
        // POST: /Fixture/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Fixture fixture)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fixture).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fixture);
        }

        //
        // GET: /Fixture/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Fixture fixture = db.Fixtures.Find(id);
            if (fixture == null)
            {
                return HttpNotFound();
            }
            return View(fixture);
        }

        //
        // POST: /Fixture/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Fixture fixture = db.Fixtures.Find(id);
            db.Fixtures.Remove(fixture);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Tworzy terminarz rozgrywek
        /// </summary>
        /// <returns>Widok</returns>
        
        [Authorize(Roles = "admin")]
        public ActionResult CreateFixtures()
        {



            return View("../Admin/Fixture/CreateFixtures", new Fixture());
        }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}