using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LechTyper.Models;
using LechTyper.Repository;

namespace LechTyper.Controllers
{
    public class FixtureController : Controller
    {
        private FixtureContext dbFixture = new FixtureContext();
        private LeagueContext dbLeague = new LeagueContext();
        private UsersContext dbUser = new UsersContext();
        private FixtureRepository _fixtureRepository;
        private LeagueRepository _leagueRepository;
        private MainRepository _mainRepository;

        public FixtureController()
        {
            _leagueRepository = new LeagueRepository(dbLeague);
            _fixtureRepository = new FixtureRepository(dbFixture);
            _mainRepository = new MainRepository(dbUser);
        }

        /// <summary>
        /// Wyświetlanie najlbiższej kolejki spotkań
        /// </summary>
        /// <returns>Widok</returns>
        public ActionResult MatchDayDisplay(bool nextMatchDay = true)
        {
            int matchDay;
            if (nextMatchDay)
                matchDay = _fixtureRepository.NextMatchDay();
            else
                matchDay = _fixtureRepository.LastMatchDay();
            var fixtureList = _fixtureRepository.GetFixturesByMatchDay(matchDay);
            var userIdList = _fixtureRepository.GetUserIdByFixture(fixtureList);
            Dictionary<int, string> userNameDictionary = new Dictionary<int, string>();
            foreach (var user in userIdList)
            {
                userNameDictionary.Add(user, dbUser.UserProfiles.Where(u => u.UserId == user).Select(u => u.UserName).FirstOrDefault());
            }
            ViewBag.MatchDay = matchDay;
            ViewBag.userNameDictionary = userNameDictionary;
            return View(fixtureList);
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
        public string CreateNewFixture()
        {
            try
            {
                var divisionsList = _leagueRepository.GetDivisions();
                var playerList = _leagueRepository.GetLeagues();
                List<Fixture> fixtureDivision = new List<Fixture>();
                foreach (var division in divisionsList)
                {
                    CreateFixtureForDivision(ref fixtureDivision, playerList.Where(d => d.division == division).OrderBy(u => u.userId).Select(u => u.userId).ToList());
                }

                foreach (var match in fixtureDivision)
                {
                    if (!dbFixture.Fixtures.Any(x => x.homeId == match.homeId && x.guestId == match.guestId && x.matchDay == match.matchDay))
                        dbFixture.Fixtures.Add(match);
                }
                dbFixture.SaveChanges();
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Wywołuję metodę tworzenia terminarza
        /// </summary>
        /// <returns>Widok</returns>   
         [Authorize(Roles = "admin")]
        public ActionResult CreateFixtures()
        {
            var response = CreateNewFixture();
            if(response != "OK")
                return RedirectToAction("Error", "Error", response);

            return View("../Admin/Fixture/CreateFixtures", dbFixture.Fixtures.Local.OrderBy(x => x.id).ThenBy(x => x.matchDay).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            dbFixture.Dispose();
            base.Dispose(disposing);
        }
    }
}