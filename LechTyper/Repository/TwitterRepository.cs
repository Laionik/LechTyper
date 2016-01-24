using LechTyper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LechTyper.Repository
{

    public class TwitterRepository
    {
        private TwitterContext dbTwitter;
        private MatchRepository _matchRepository;

        public TwitterRepository()
        {
            this.dbTwitter = new TwitterContext();
            _matchRepository = new MatchRepository(new MatchContext());
        }
        public TwitterRepository(TwitterContext dbTwitter)
        {
            _matchRepository = new MatchRepository(new MatchContext());
            this.dbTwitter = dbTwitter;
        }

        /// <summary>
        /// Pobieranie wszystkich tweetów
        /// </summary>
        /// <returns>Lista tweetów</returns>
        public List<Tweet> GetTweets()
        {
            return dbTwitter.Tweets.ToList();
        }

        /// <summary>
        /// Wyszukiwanie tweetu po ID
        /// </summary>
        /// <returns>Tweet</returns>
        public Tweet GetTweetById(int id)
        {
            return dbTwitter.Tweets.Where(m => m.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Wyszukiwanie tweetu po nazwie użytkownika
        /// </summary>
        /// <returns>Tweet</returns>
        public Tweet GetTweetByUserName(string userName)
        {
            return dbTwitter.Tweets.Where(m => m.userName == userName).FirstOrDefault();
        }

        /// <summary>
        /// Wypisanie postu o meczu na Twittera
        /// </summary>
        public void PostMatchTweet(int days, string address)
        {
            var nextMatch = _matchRepository.GetNextMatch();
            var tag = "#lechtypertest";
            StringBuilder sb = new StringBuilder();
            if (days == 0)
                sb.AppendFormat("{0}:{1} już dzisiaj! Lista spotkań {2} {3}", nextMatch.host, nextMatch.guest, address, tag);
            else if (days == 1)
                sb.AppendFormat("{0}:{1} już za jutro! Lista spotkań {2} {3}", nextMatch.host, nextMatch.guest, address, tag);
            else
                sb.AppendFormat("{0}:{1} już za {2} dni! Lista spotkań {3} {4}", nextMatch.host, nextMatch.guest, nextMatch.date.Subtract(DateTime.Now).Days, address, tag);

            Tweetinvi.Tweet.PublishTweet(sb.ToString());
        }
        /// <summary>
        /// Wypisanie postu po meczu na Twittera
        /// </summary>
        public void PostAfterMatchTweet(string address)
        {
            var tag = "#lechtypertest";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Sprawdźcie najnowsze wyniki! {0} {1}", address, tag);
            Tweetinvi.Tweet.PublishTweet(sb.ToString());
        }

        /// <summary>
        /// Wypisanie postu na Twittera
        /// </summary>
        public void PostTweet(int number)
        {
            Tweetinvi.Tweet.PublishTweet(DateTime.Now.ToString() + " wartość testowa " + number);
            //var nextMatch = _matchRepository.GetNextMatch();
            //var request = HttpContext.Request;
            //var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
            //var tag = "#lechtypertest";
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("{0} : {1} już za {2} dni! Lista spotkań {3}. Zapraszam do typowania {4}", nextMatch.host, nextMatch.guest, nextMatch.date.Subtract(DateTime.Now).Days, address + Url.Action("CurrentMatchDayDisplay", "Fixture"), tag);
            //Tweetinvi.Tweet.PublishTweet(sb.ToString());
        }
    }
}