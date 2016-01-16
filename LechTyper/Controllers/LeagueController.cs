using LechTyper.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace LechTyper.Controllers
{
    public class LeagueController : Controller
    {
        private LeagueContext dbLeague = new LeagueContext();
        private UsersContext dbUser = new UsersContext();

        //
        // GET: /League/

        public ActionResult Index()
        {
            return View(dbLeague.Leagues.ToList());
        }

        //
        // GET: /League/Details/5

        public ActionResult Details(int id = 0)
        {
            League league = dbLeague.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        //
        // GET: /League/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /League/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(League league)
        {
            if (ModelState.IsValid)
            {
                dbLeague.Leagues.Add(league);
                dbLeague.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(league);
        }

        //
        // GET: /League/Edit/5

        public ActionResult Edit(int id = 0)
        {
            League league = dbLeague.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        //
        // POST: /League/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(League league)
        {
            if (ModelState.IsValid)
            {
                dbLeague.Entry(league).State = EntityState.Modified;
                dbLeague.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(league);
        }

        //
        // GET: /League/Delete/5

        public ActionResult Delete(int id = 0)
        {
            League league = dbLeague.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        //
        // POST: /League/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            League league = dbLeague.Leagues.Find(id);
            dbLeague.Leagues.Remove(league);
            dbLeague.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// System awansów i spadków
        /// </summary>
        /// <returns>Widok</returns>
        public ActionResult LeaguePromotions()
        {
            if (dbLeague.Leagues.Any(x => x.matches >= 10))
            {
                try
                {
                    var lowestDivision = dbLeague.Leagues.OrderByDescending(x => x.division).Select(d => d.division).FirstOrDefault();
                    List<League> promotionList = new List<League>();
                    List<League> relegationList = new List<League>();

                    for (int division = 1; division <= lowestDivision; division++)
                    {
                        if (division != 1)
                            promotionList = dbLeague.Leagues.Where(d => d.division == division).OrderByDescending(x => x.points).ThenByDescending(g => g.goalScored - g.goalConceded).ThenByDescending(g => g.goalScored).Take(2).ToList();

                        var leagueCount = dbLeague.Leagues.Where(d => d.division == division).Count();
                        if (leagueCount > 8)
                        {
                            var lowerLeagueCount = dbLeague.Leagues.Where(d => d.division == division + 1).Count();
                            relegationList = dbLeague.Leagues.Where(d => d.division == division).OrderBy(x => x.points).ThenBy(g => g.goalScored - g.goalConceded).ThenBy(g => g.goalScored).Take(lowerLeagueCount > 2 ? 2 : lowerLeagueCount).ToList();
                        }

                        foreach (var promo in promotionList)
                        {
                            promo.division -= 1;
                            TryUpdateModel(promo);
                        }

                        foreach (var releg in relegationList)
                        {
                            releg.division += 1;
                            TryUpdateModel(releg);
                        }
                    }

                    dbLeague.SaveChanges();
                }
                catch (Exception e)
                {
                    return RedirectToAction("DatabaseError", "Error", e.Message);
                }
            }

            return View("../Admin/League/LeaguePromotions", dbLeague.Leagues.Local.ToList());
        }


        /// <summary>
        /// Aktualizacja składu lig
        /// </summary>
        /// <returns>Widok</returns>
        [Authorize(Roles = "admin")]
        public ActionResult LeagueAddUsers()
        {
            try
            {
                var leagueUserList = dbLeague.Leagues.Select(x => x.userId).ToList();
                var lowestDivision = dbLeague.Leagues.OrderByDescending(x => x.division).Select(d => d.division).FirstOrDefault();
                if (lowestDivision == 0)
                    lowestDivision = 1;
                var lowestDivisionCount = dbLeague.Leagues.Where(d => d.division == lowestDivision).Count();
                var userList = dbUser.UserProfiles.Where(x => !(leagueUserList.Contains(x.UserId))).ToList().Select(u => u.UserId).ToList();
                foreach (var user in userList)
                {
                    dbLeague.Leagues.Add(new League(user, lowestDivision));
                    if ((dbLeague.Leagues.Local.Count() + lowestDivisionCount) % 10 == 0)
                        lowestDivision++;
                }
                dbLeague.SaveChanges();
                return View("../Admin/League/LeagueAddUsers", dbLeague.Leagues.Local.ToList());
            }
            catch (Exception e)
            {
                return RedirectToAction("DatabaseError", "Error", e.Message);
            }
        }
        protected override void Dispose(bool disposing)
        {
            dbLeague.Dispose();
            base.Dispose(disposing);
        }
    }
}