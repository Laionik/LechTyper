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

namespace LechTyper.Controllers
{
    public class TwitterController : Controller
    {
        TwitterContext db = new TwitterContext();
        //
        // GET: /Twitter/
        public async Task<ActionResult> Twitter()
        {
            Tweet.PublishTweet("Hello World!");
            var twitts = await RunClient();
            if (twitts != null)
            {
                foreach (var x in twitts)
                {
                    Twitt twittsData = db.Twitts.FirstOrDefault(u => u.post_id.ToLower() == x.post_id.ToLower());
                    try
                    {
                        if (twittsData == null)
                        {
                            db.Twitts.Add(x);
                            db.SaveChanges();
                        }
                        else if (TryUpdateModel(x))
                        {

                            db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("DatabaseError", "Error");
                    }
                }
                return View(twitts);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

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

        async Task<List<Twitt>> RunClient()
        {
            string _address = "https://api.twitter.com/1.1/search/tweets.json?q=ltlecpog&count=100";
            HttpClient client = new HttpClient(new OAuthMessageHandler(new HttpClientHandler()));
            HttpResponseMessage response = await client.GetAsync(_address);
            List<Twitt> twitts = new List<Twitt>();
            long last = 0;
            string text = "";
            if (response.IsSuccessStatusCode)
            {
                JToken statuses = await response.Content.ReadAsAsync<JToken>();
                foreach (var x in statuses["statuses"])
                {
                    if (TextValidate(x["text"].ToString()))
                    {
                        text = Regex.Replace(text, @"@lechtyperdev\s*", "");
                        text = x["text"].ToString().Replace("-", ":");
                        twitts.Add(new Twitt(x["created_at"].ToString(), x["id_str"].ToString(), text.ToLower(), x["user"]["id_str"].ToString(), x["user"]["name"].ToString(), x["user"]["screen_name"].ToString()));
                    }
                    last = long.Parse(x["id_str"].ToString()) - 1;
                }

                do
                {
                    _address = "https://api.twitter.com/1.1/search/tweets.json?q=ltlecpog&count=100&max_id=" + last.ToString();
                    response = await client.GetAsync(_address);
                    if (response.IsSuccessStatusCode)
                    {
                        statuses = await response.Content.ReadAsAsync<JToken>();
                        if (statuses["statuses"].Count() == 0)
                            break;
                        else
                        {
                            foreach (var x in statuses["statuses"])
                            {
                                if (TextValidate(x["text"].ToString()))
                                {
                                    text = x["text"].ToString().Replace("@lechtyperdev", "");
                                    twitts.Add(new Twitt(x["created_at"].ToString(), x["id_str"].ToString(), text.ToLower(), x["user"]["id_str"].ToString(), x["user"]["name"].ToString(), x["user"]["screen_name"].ToString()));
                                }
                                last = long.Parse(x["id_str"].ToString()) - 1;
                            }
                        }
                    }
                } while (true);
                return twitts;
            }
            return null;
        }
    }
}