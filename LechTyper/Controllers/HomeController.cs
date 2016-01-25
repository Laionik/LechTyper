using Hangfire;
using LechTyper.Models;
using LechTyper.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LechTyper.Controllers
{
    public class HomeController : Controller
    {
        private UsersContext dbUser;
        private TwitterController _twitterController;
        private MatchController _matchController;
        private MatchRepository _matchRepository;
        private TwitterRepository _twitterRepository;
        private LeagueController _leagueController;
        private List<string> competitionList;
        public HomeController()
        {
            dbUser = new UsersContext();
            competitionList = new List<string>();
            competitionList.Add("Ekstraklasa");
            competitionList.Add("PolishCup");
            competitionList.Add("Supercup");
            competitionList.Add("EuropaLeague");
            competitionList.Add("ChampionsLeague");
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }


        /// <summary>
        /// Prowadzenie gry LechTyper
        /// </summary>
        public void Play()
        {
            try
            {
                _matchController = new MatchController();
                _matchRepository = new MatchRepository(new MatchContext());
                _twitterRepository = new TwitterRepository(new TwitterContext());
                _twitterController = new TwitterController();
                _leagueController = new LeagueController();
                foreach(var competition in competitionList)
                {
                    var response = _matchController.UpdateDatabaseCompetition(competition);
                }

                var nextMatchIn = _matchRepository.GetDaysToNextMatch();
                var request = HttpContext.Request;
                var address = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, Url.Action("CurrentMatchDayDisplay", "Fixture"));

               if(nextMatchIn <= 3)
                {       
                    _twitterRepository.PostMatchTweet(nextMatchIn, address);
                    var response = _twitterController.TwitterSearchPosts();
                   // if (response != "OK")
                   //     SEND MAIL
                }
               else if(_matchRepository.GetLastMatchDays() == -1)
               {
                   var lastMatchDate = _matchRepository.GetLastMatch().date;
                   var responseSearch = _twitterController.TwitterSearchPosts(lastMatchDate);
                   var responseLeague = _leagueController.ProcessResults();
                   _twitterRepository.PostAfterMatchTweet(address);
               }

            }
            catch (Exception e)
            {
                RedirectToAction("Error", "Error", e.Message);
            }
        }
    }
}
