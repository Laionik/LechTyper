using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using LechTyper.OAuth;
using System.Net.Http;
using System.Threading.Tasks;
using LechTyper.Models;
using System.Text.RegularExpressions;
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

        // GET: /Twitter/
        public ActionResult Twitter()
        {
            //Tweet.PublishTweet("Aktualizuję wpisy!");
            var tweetList = Search.SearchTweets("lechtyperdev").GroupBy(t => t.CreatedBy.ScreenName).Select(g => g.First()).ToList();

            var userList = GetUsersNames().Where(u => !(tweetList.Select(t => t.CreatedBy.ScreenName).Contains(u)));
            foreach (var user in userList)
            {
                var tweetTemp = Timeline.GetUserTimeline(user).FirstOrDefault(t => t.Text.Contains("lechtyperdev"));
                if (tweetTemp != null)
                    tweetList.Add(tweetTemp);
            }

            foreach (var tweet in tweetList)
            {
                tweet.Text = TextCorrecting(tweet.Text);
            }

            var result = TwitterUpdate(tweetList);

            return View(tweetList);
        }


        public List<string> GetUsersNames()
        {
            return dbUser.UserProfiles.Select(u => u.UserName).ToList();
        }


        // Sprawdź poprawnosć typu
        public bool TextValidate(string text)
        {
            Regex form1 = new Regex(@"^\s*[0-9]+-[0-9]+");
            Regex form2 = new Regex(@"^\s*[0-9]+:[0-9]+");
            text = Regex.Replace(text, @"@lechtyperdev\s*", "");
            if (form1.IsMatch(text) || form2.IsMatch(text))
                return true;
            else
                return false;
        }

        //usuwanie zbędnych wiadomości z typu
        public string TextCorrecting(string text)
        {
            text = new Regex(@"@[a-zA-Z0-9_]*\s*").Replace(text, "");
            text = new Regex(@"(\s*[:]\s*)|(\s*[-]\s*)").Replace(text, ":");
            text = new Regex(@"[#][a-zA-Z]*").Replace(text, "");
            text = new Regex(@"[.,\-\/#!$%\^&\*;{}=\-_`~()]").Replace(text, " ");
            text = new Regex(@"\s+").Replace(text, " ");
            return text;
        }

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
                        //update
                        tweetNew.result = tweetTemp[0];
                        tweetNew.resultHalf = tweetTemp[1];
                        tweetNew.scorer = tweetTemp[2];
                        TryUpdateModel(tweetNew);
                    }
                    else
                    {
                        //add
                        tweetNew = new LechTyper.Models.Tweet(tweet.CreatedBy.ScreenName, tweetTemp[0], tweetTemp[1], tweetTemp[2], DateTime.Parse(tweet.CreatedAt.ToShortDateString()));
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
    }
}