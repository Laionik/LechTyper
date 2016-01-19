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
        private FixtureContext dbFixture = new FixtureContext();
        private LeagueContext dbLeague = new LeagueContext();

        //
        // GET: /Fixture/

        public ActionResult Index()
        {
            return View(dbFixture.Fixtures.ToList());
        }

        //
        // GET: /Fixture/Details/5

        public ActionResult Details(int id = 0)
        {
            Fixture fixture = dbFixture.Fixtures.Find(id);
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
                dbFixture.Fixtures.Add(fixture);
                dbFixture.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fixture);
        }

        //
        // GET: /Fixture/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Fixture fixture = dbFixture.Fixtures.Find(id);
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
                dbFixture.Entry(fixture).State = EntityState.Modified;
                dbFixture.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fixture);
        }

        //
        // GET: /Fixture/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Fixture fixture = dbFixture.Fixtures.Find(id);
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
            Fixture fixture = dbFixture.Fixtures.Find(id);
            dbFixture.Fixtures.Remove(fixture);
            dbFixture.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Tworzy terminarz dla danego poziomu rozgrywek
        /// </summary>
        /// <param name="fixtureDivision">Terminarz - uzupełnianie listy</param>
        /// <param name="players">Lista graczy w lidze</param>
        public void CreateFixtureForDivision(ref List<Fixture> fixtureDivision, List<int> players)
        {
            int numDays = players.Count - 1;
            int halfSize = players.Count / 2;

            List<int> teams = new List<int>();

            teams.AddRange(players);
            teams.RemoveAt(0);

            int teamsSize = teams.Count;

            for (int day = 0; day < numDays; day++)
            {
                int teamIdx = day % teamsSize;
                fixtureDivision.Add(new Fixture(day + 1, teams[teamIdx], players[0]));

                for (int idx = 1; idx < halfSize; idx++)
                {
                    int firstTeam = (day + idx) % teamsSize;
                    int secondTeam = (day + teamsSize - idx) % teamsSize;
                    fixtureDivision.Add(new Fixture(day + 1, teams[firstTeam], teams[secondTeam]));
                }
            }
        }
        
        /// <summary>
        /// Tworzy terminarz rozgrywek
        /// </summary>
        /// <returns>Widok</returns>   
        // [Authorize(Roles = "admin")]
        public ActionResult CreateFixtures()
        {
            var divisionsList = dbLeague.Leagues.Select(d => d.division).Distinct().ToList();
            var playerList = dbLeague.Leagues.OrderBy(d => d.division).ToList();
            List<Fixture> fixtureDivision = new List<Fixture>();
            foreach(var division in divisionsList)
            {
                CreateFixtureForDivision(ref fixtureDivision, playerList.Where(d => d.division == division).OrderBy(u => u.userId).Select(u => u.userId).ToList());
            }

            foreach(var match in fixtureDivision)
            {
                if (!dbFixture.Fixtures.Any(x => x.homeId == match.homeId && x.guestId == match.guestId && x.matchDay == match.matchDay))
                    dbFixture.Fixtures.Add(match);
            }
            dbFixture.SaveChanges();
            return View("../Admin/Fixture/CreateFixtures", dbFixture.Fixtures.Local.OrderBy(x => x.id).ThenBy(x => x.matchDay).ToList());
        }



        protected override void Dispose(bool disposing)
        {
            dbFixture.Dispose();
            base.Dispose(disposing);
        }
    }
}