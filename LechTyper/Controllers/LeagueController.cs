using LechTyper.Filters;
using LechTyper.Models;
using LechTyper.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace LechTyper.Controllers
{
    [InitializeSimpleMembership]
    public class LeagueController : Controller
    {
        private LeagueContext dbLeague;
        private UsersContext dbUser;
        private MatchContext dbMatch;
        private TwitterContext dbTwitter;
        private FixtureContext dbFixture;
        private FixtureRepository _fixtureRepository;
        private LeagueRepository _leagueRepository;
        private MainRepository _mainRepository;
        private MatchRepository _matchRepository;
        private FixtureController _fixtureController;

        public LeagueController()
        {
            dbLeague = new LeagueContext();
            dbUser = new UsersContext();
            dbMatch = new MatchContext();
            dbTwitter = new TwitterContext();
            dbFixture = new FixtureContext();
            _fixtureRepository = new FixtureRepository(dbFixture);
            _leagueRepository = new LeagueRepository(dbLeague);
            _mainRepository = new MainRepository(dbUser);
            _matchRepository = new MatchRepository(dbMatch);
            _fixtureController = new FixtureController();
        }

        /// <summary>
        /// Wyświetlanie tabeli ligowej
        /// </summary>
        /// <param name="division">Poziom rozgrywek</param>
        /// <returns>Widok</returns>
        public ActionResult LeagueDisplay(int? division)
        {
            try
            {
                if (!division.HasValue)
                {
                    if (User.Identity.Name != String.Empty)
                    {
                        int userId = WebSecurity.GetUserId(User.Identity.Name);
                        division = dbLeague.Leagues.Where(u => u.userId == userId).Select(d => d.division).FirstOrDefault();
                    }
                    else
                        division = 1;
                }

                ViewBag.Leagues = dbLeague.Leagues.Select(d => d.division).Distinct().ToList();
                var leagueList = dbLeague.Leagues.Where(d => d.division == division).OrderByDescending(x => x.points).ThenByDescending(g => g.goalScored - g.goalConceded).ThenByDescending(g => g.goalScored).ToList();
                Dictionary<int, string> userNameDictionary = new Dictionary<int, string>();
                foreach (var user in leagueList.Select(x => x.userId))
                {
                    userNameDictionary.Add(user, dbUser.UserProfiles.Where(u => u.UserId == user).Select(u => u.UserName).FirstOrDefault());
                }
                ViewBag.division = division;
                ViewBag.userNameDictionary = userNameDictionary;
                return View(leagueList);
            }
            catch (Exception e)
            {

                return RedirectToAction("Error", "Error", e.Message);
            }
        }

        /// <summary>
        /// System awansów i spadków
        /// </summary>
        /// <returns>Wynik operacji</returns>
        public string LeagueNewSeasonPromotion()
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
                        dbLeague.Leagues.Attach(promo);
                        promo.division -= 1;
                    }

                    foreach (var releg in relegationList)
                    {
                        dbLeague.Leagues.Attach(releg);
                        releg.division += 1;
                    }
                }

                dbLeague.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// System awansów i spadków
        /// </summary>
        /// <returns>Widok</returns>
        [Authorize(Roles = "admin")]
        public ActionResult LeaguePromotions()
        {
            var response = LeagueNewSeasonPromotion();
            if (response != "OK")
                return RedirectToAction("Error", "Error", response);

            return View("../Admin/League/LeaguePromotions", dbLeague.Leagues.Local.ToList());
        }


        /// <summary>
        /// Aktualizacja składów lig
        /// </summary>
        /// <returns>Wynik dodawania użytkowników</returns>
        public string LeagueNewSeasonAddUsers()
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
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        /// <summary>
        /// Aktualizacja składu lig
        /// </summary>
        /// <returns>Widok</returns>
        [Authorize(Roles = "admin")]
        public ActionResult LeagueAddUsers()
        {
            var response = LeagueNewSeasonAddUsers();
            if (response != "OK")
                return RedirectToAction("Error", "Error", response);
            return View("../Admin/League/LeagueAddUsers", dbLeague.Leagues.Local.ToList());
        }

        /// <summary>
        /// Ekstrakcja wyniku ze strina
        /// </summary>
        /// <param name="result">String z wynikiem</param>
        /// <param name="hostGoal">Bramki gospodarzy</param>
        /// <param name="guestGoal">Bramki gości</param>
        private void ResultTweet(string result, out int hostGoal, out int guestGoal)
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
        private int CalculateGoals(int hostPrediction, int guestPrediction, int hostGoal, int guestGoal)
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
        public void CheckTweetResult(Tweet tweet, Match match, Fixture playerFixture, int userId)
        {
            if (tweet != null)
            {
                int hostGoal, guestGoal, playerGoal = 0;
                ResultTweet(tweet.result, out hostGoal, out guestGoal);
                playerGoal = CalculateGoals(hostGoal, guestGoal, match.finalHostGoal, match.finalGuestGoal);
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
        }

        /// <summary>
        /// Czyszczenie tabel bazy danych
        /// </summary>
        private void ClearTables()
        {
            //dbLeague.Database.ExecuteSqlCommand("TRUNCATE TABLE [League]");
            dbFixture.Database.ExecuteSqlCommand("TRUNCATE TABLE [Fixture]");
        }

        /// <summary>
        /// Przygotowywanie nowego sezonu
        /// </summary>
        public void PrepareNewSeason()
        {
            //saveOldTables
            ClearTables();
            var responsePromotion = LeagueNewSeasonPromotion();
            var leagueList = dbLeague.Leagues.ToList();
            foreach (var league in leagueList)
            {
                dbLeague.Leagues.Attach(league);
                league.draw = 0;
                league.goalConceded = 0;
                league.goalScored = 0;
                league.lose = 0;
                league.matches = 0;
                league.points = 0;
                league.win = 0;
            }
            dbLeague.SaveChanges();
            var addUserResponse = LeagueNewSeasonAddUsers();
            var fixtureResponse = _fixtureController.CreateNewFixture();
            //if all ok - send mail
        }

        /// <summary>
        /// Przetwarzanie wyników
        /// </summary>
        /// <returns>Status przetwarzania</returns>
        public string ProcessResults()
        {
            var match = _matchRepository.GetLastMatch();
            var tweetList = dbTwitter.Tweets.ToList();
            var matchDay = _fixtureRepository.NextMatchDay();
            var fixtureList = dbFixture.Fixtures.Where(f => f.matchDay == matchDay).ToList();

            var isSeasonCompleted = matchDay >= 8 ? true : false;
            string userName = "";
            try
            {
                foreach (var fixture in fixtureList)
                {
                    dbFixture.Fixtures.Attach(fixture);

                    userName = dbUser.UserProfiles.Where(u => u.UserId == fixture.homeId).Select(u => u.UserName).FirstOrDefault();
                    CheckTweetResult(dbTwitter.Tweets.Where(t => t.userName == userName).FirstOrDefault(), match, fixture, fixture.homeId);

                    userName = dbUser.UserProfiles.Where(u => u.UserId == fixture.guestId).Select(u => u.UserName).FirstOrDefault();
                    CheckTweetResult(dbTwitter.Tweets.Where(t => t.userName == userName).FirstOrDefault(), match, fixture, fixture.guestId);

                    var host = dbLeague.Leagues.Where(u => u.userId == fixture.homeId).FirstOrDefault();
                    var guest = dbLeague.Leagues.Where(u => u.userId == fixture.guestId).FirstOrDefault();
                    dbLeague.Leagues.Attach(host);
                    dbLeague.Leagues.Attach(guest);

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

                    if (host.matches >= 8 || guest.matches >= 8)
                        isSeasonCompleted = true;
                }

                dbFixture.SaveChanges();
                dbLeague.SaveChanges();
                if (isSeasonCompleted)
                    PrepareNewSeason();
                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR";

            }
        }

        /// <summary>
        /// Generowanie wyników spotkań
        /// </summary>
        /// <returns>Widok</returns>
        public ActionResult LeagueResults()
        {
            var response = ProcessResults();
            var leagueList = _leagueRepository.GetLeagues();
            return View(leagueList);

        }

        protected override void Dispose(bool disposing)
        {
            dbLeague.Dispose();
            base.Dispose(disposing);
        }
    }
}