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
        private MatchContext dbMatch = new MatchContext();
        private TwitterContext dbTwitter = new TwitterContext();
        private FixtureContext dbFixture = new FixtureContext();


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


        /// <summary>
        /// Ekstrakcja wyniku ze strina
        /// </summary>
        /// <param name="result">String z wynikiem</param>
        /// <param name="hostGoal">Bramki gospodarzy</param>
        /// <param name="guestGoal">Bramki gości</param>
        private void resultTweet(string result, out int hostGoal, out int guestGoal)
        {
            var temp = result.Split(':');
            hostGoal = int.Parse(temp[0]);
            guestGoal = int.Parse(temp[1]);
        }

        /// <summary>
        /// Obliczanie wyniku użytkownika
        /// </summary>
        /// <param name="hostPrediction">Przewidywana liczba bramek gospodarzy</param>
        /// <param name="guestPrediction">Przewidywana liczba bramek gości</param>
        /// <param name="hostGoal">Liczba bramek gospodarzy</param>
        /// <param name="guestGoal">Liczba bramek gości</param>
        /// <returns>Liczba punktów zdobytych przez gracza</returns>
        private int calculateGoals(int hostPrediction, int guestPrediction, int hostGoal, int guestGoal)
        {
            int playerGoal = 0;
            if (hostPrediction == hostGoal && guestPrediction == guestGoal)
                playerGoal += 1;
            if (hostPrediction == hostGoal || guestPrediction == guestGoal)
                playerGoal += 1;
            if (((hostPrediction - guestPrediction) < 0 && (hostGoal - guestGoal) < 0) || ((hostPrediction - guestPrediction) > 0 && (hostGoal - guestGoal) > 0) || ((hostPrediction - guestPrediction) == 0 && (hostGoal - guestGoal) == 0))
                playerGoal += 1;
            return playerGoal;
        }

        /// <summary>
        /// Sprawdzanie typu użytkownika
        /// </summary>
        /// <param name="tweet">Post</param>
        /// <param name="match">Mecz Lecha</param>
        /// <param name="playerFixture">Mecz typerów</param>
        /// <param name="userId">Id użytkownika</param>
        public void checkTweetResult(Tweet tweet, Match match, Fixture playerFixture, int userId)
        {
            if (tweet != null)
            {
                //    var userId = dbUser.UserProfiles.Where(u => u.UserName == tweet.userName).Select(u => u.UserId).FirstOrDefault();
                int hostGoal, guestGoal, playerGoal = 0;
                resultTweet(tweet.result, out hostGoal, out guestGoal);
                playerGoal = calculateGoals(hostGoal, guestGoal, match.finalHostGoal, match.finalGuestGoal);
                //var playerFixture = fixtureList.Where(f => f.homeId == userId || f.guestId == userId).FirstOrDefault();
                if (playerFixture.homeId == userId)
                    playerFixture.homeGoal = playerGoal;
                else
                    playerFixture.guestGoal = playerGoal;
            }
            else
            {
                if (playerFixture.homeId == userId)
                    playerFixture.homeGoal = 0;
                else
                    playerFixture.guestGoal = 0;
            }
            TryUpdateModel(playerFixture);
        }

        /// <summary>
        /// Generowanie wyników spotkań
        /// </summary>
        /// <returns>Widok</returns>
        public ActionResult LeagueResults()
        {
            var match = dbMatch.MatchData.Where(m => m.isCompleted).OrderBy(m => m.date).ToList().Last();
            var tweetList = dbTwitter.Tweets.ToList();
            var matchDay = dbFixture.Fixtures.Where(f => f.homeGoal == null).Select(f => f.matchDay).FirstOrDefault();
            var fixtureList = dbFixture.Fixtures.Where(f => f.matchDay == matchDay).ToList();

            var isSeasonCompleted = false;
            string userName = "";
            try
            {
                foreach (var fixture in fixtureList)
                {
                    userName = dbUser.UserProfiles.Where(u => u.UserId == fixture.homeId).Select(u => u.UserName).FirstOrDefault();
                    checkTweetResult(dbTwitter.Tweets.Where(t => t.userName == userName).FirstOrDefault(), match, fixture, fixture.homeId);

                    userName = dbUser.UserProfiles.Where(u => u.UserId == fixture.guestId).Select(u => u.UserName).FirstOrDefault();
                    checkTweetResult(dbTwitter.Tweets.Where(t => t.userName == userName).FirstOrDefault(), match, fixture, fixture.guestId);

                    var host = dbLeague.Leagues.Where(u => u.userId == fixture.homeId).FirstOrDefault();
                    var guest = dbLeague.Leagues.Where(u => u.userId == fixture.guestId).FirstOrDefault();
                    if (fixture.homeGoal > fixture.guestGoal)
                    {
                        host.points += 3;
                        host.win += 1;
                        guest.lose += 1;
                    }
                    else if (fixture.homeGoal == fixture.guestGoal)
                    {
                        host.points += 1;
                        host.draw += 1;
                        guest.draw += 1;
                        guest.points += 1;
                    }
                    else
                    {
                        guest.points += 3;
                        host.lose += 1;
                        guest.win += 1;
                    }

                    host.matches += 1;
                    guest.matches += 1;

                    TryUpdateModel(host);
                    TryUpdateModel(guest);
                    if (host.matches >= 9 || guest.matches >= 9)
                        isSeasonCompleted = true;
                }

                dbFixture.SaveChanges();
                dbLeague.SaveChanges();
                //if(isSeasonCompleted)
                //Nowy sezon

                return View(dbLeague.Leagues.Local.ToList());
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