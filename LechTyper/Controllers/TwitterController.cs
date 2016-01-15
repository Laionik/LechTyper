using LechTyper.Models;
using LechTyper.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tweetinvi;
using Tweetinvi.Core.Interfaces;

namespace LechTyper.Controllers
{
    public class TwitterController : Controller
    {
        private UsersContext dbUser;
        private TwitterContext dbTwitter;
        public TwitterController()
        {
            dbUser = new UsersContext();
            dbTwitter = new TwitterContext();
        }
        ///<summary>
        ///GET: /Twitter/
        ///</summary>
        ///<returns>Widok</returns>
        public ActionResult Twitter()
        {
            string hastag = "#lechtyperdev1";
            var tweetStartList = Search.SearchTweets(hastag).GroupBy(t => t.CreatedBy.ScreenName).Select(g => g.First()).ToList();
            var userList = GetUsersNames().Where(u => !(tweetStartList.Select(t => t.CreatedBy.ScreenName).Contains(u)));
            foreach (var user in userList)
            {
                if (Tweetinvi.User.GetUserFromScreenName(user) != null)
                {
                    var tweetTemp = Timeline.GetUserTimeline(user).FirstOrDefault(t => t.Text.Contains(hastag));
                    if (tweetTemp != null)
                    {
                        tweetStartList.Add(tweetTemp);
                    }
                    else
                        Tweetinvi.Tweet.PublishTweet("@" + user + " nie zapomniałeś o nas? " + hastag);
                }
            }
            //until=2016-01-14 <= Dorzuć do filtrowania postów
            List<ITweet> tweetList = new List<ITweet>();
            foreach (var tweet in tweetStartList)
            {
                TextValidate(tweet, ref tweetList);
            }

            var result = TwitterUpdate(tweetList);

            List<LechTyper.Models.Tweet> tweetAllList = GetAllTweets();
            return View(tweetAllList);
        }

        ///<summary>
        ///Pobieranie wszystkich tweetów
        ///</summary>
        ///<returns>Lista tweetów z bazy</returns>
        public List<LechTyper.Models.Tweet> GetAllTweets()
        {
            return dbTwitter.Tweets.ToList();
        }

        ///<summary>
        ///Pobieranie listy nazw użytkowników
        ///</summary>
        ///<returns>Lista nazw użytkowników z bazy danych</returns>
        public List<string> GetUsersNames()
        {
            return dbUser.UserProfiles.Select(u => u.UserName).ToList();
        }

        ///<summary>
        ///Sprawdzanie poprawnosć typu
        ///</summary>
        ///<param name="tweet">Tweet do sprawdzenia</param>
        ///<param name="tweetList">Lista tweetów</param>
        public void TextValidate(ITweet tweet, ref List<ITweet> tweetList)
        {
            Regex predictForm = new Regex(@"^\s*([0-9]+:[0-9]+\s*){2}\s*[a-zA-Z]+\s*$");
            tweet.Text = TextCorrecting(tweet.Text);
            if (predictForm.IsMatch(RemovePolishLetters(tweet.Text)))
            {
                tweetList.Add(tweet);
                Tweetinvi.Tweet.FavoriteTweet(tweet.TweetDTO.Id);
            }
            else
            {
                var textToPublish = string.Format("@{0} {1}", tweet.CreatedBy.ScreenName, "Twój typ jest niepoprawny. Typuj zgodnie ze wzorem!");
                Tweetinvi.Tweet.PublishTweetInReplyTo(textToPublish, tweet.TweetDTO.Id);
            }
        }
        ///<summary>
        ///Usuwanie zbędnych wiadomości z typu
        ///</summary>
        ///<param name="text">tweet.Text do poprawienia</param>
        ///<returns>Poprawiony tweet</returns>
        public string TextCorrecting(string text)
        {
            text = new Regex(@"@[a-zA-Z0-9_]*\s*").Replace(text, "");
            text = new Regex(@"(\s*[:]\s*)|(\s*[-]\s*)").Replace(text, ":");
            text = new Regex(@"[#][a-zA-Z0-9]*").Replace(text, "");
            text = new Regex(@"[.,\-\/#!$%\^&\*;{}=\-_`~()]").Replace(text, " ");
            text = new Regex(@"\s+").Replace(text, " ");
            return text;
        }
        ///<summary>
        ///Aktualizacja bazy danych
        ///</summary>
        ///<param name="tweetsList">Lista teetów</param>
        ///<returns>Sukces lub porażkę aktualizowania bazy danych</returns>
        public bool TwitterUpdate(List<ITweet> tweetsList)
        {
            try
            {
                foreach (var tweet in tweetsList)
                {
                    var tweetTemp = tweet.Text.Split(' ');
                    var tweetNew = dbTwitter.Tweets.FirstOrDefault(t => t.userName.ToLower() == tweet.CreatedBy.ScreenName.ToLower());
                    if (tweetNew != null)
                    {
                        tweetNew.result = tweetTemp[0];
                        tweetNew.resultHalf = tweetTemp[1];
                        tweetNew.scorer = tweetTemp[2];
                        TryUpdateModel(tweetNew);
                    }
                    else
                    {
                        tweetNew = new LechTyper.Models.Tweet(tweet.TweetDTO.IdStr, tweet.CreatedBy.ScreenName, tweetTemp[0], tweetTemp[1], tweetTemp[2], DateTime.Parse(tweet.CreatedAt.ToShortDateString()));
                        dbTwitter.Tweets.Add(tweetNew);
                    }
                }
                dbTwitter.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        ///<summary>
        ///Usuwanie polskich znaków
        ///</summary>
        ///<returns>String bez polskich znaków</returns>
        public string RemovePolishLetters(string text)
        {
            return text
                .Replace("ą", "a")
                .Replace("Ą", "A")
                .Replace("ę", "e")
                .Replace("Ę", "E")
                .Replace("ś", "s")
                .Replace("Ś", "S")
                .Replace("ż", "z")
                .Replace("Ż", "Z")
                .Replace("ć", "c")
                .Replace("Ć", "C")
                .Replace("ź", "z")
                .Replace("Ź", "Z")
                .Replace("ń", "n")
                .Replace("Ń", "N")
                .Replace("ó", "o")
                .Replace("Ó", "O")
                .Replace("ł", "l")
                .Replace("Ł", "L");
        }
    }
}