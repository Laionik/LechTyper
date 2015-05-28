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

namespace LechTyper.Controllers
{
    public class TwitterController : Controller
    {
        //
        // GET: /Twitter/

        public async Task<ActionResult> Twitter()
        {
            var tweets = await RunClient();
            if (tweets != null)
            {
                return View(tweets);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        async Task<List<ReadTwitter>> RunClient()
        {
            string _address = "https://api.twitter.com/1.1/search/tweets.json?q=czterydychylecha&count=100";
            HttpClient client = new HttpClient(new OAuthMessageHandler(new HttpClientHandler()));
            HttpResponseMessage response = await client.GetAsync(_address);
            List<ReadTwitter> tweets = new List<ReadTwitter>();
            ReadTwitter previous = null;
            if (response.IsSuccessStatusCode)
            {
                JToken statuses = await response.Content.ReadAsAsync<JToken>();
                foreach (var x in statuses["statuses"])
                {
                    tweets.Add(new ReadTwitter(x["created_at"].ToString(), x["id_str"].ToString(), x["text"].ToString(), x["user"]["id_str"].ToString(), x["user"]["name"].ToString(), x["user"]["screen_name"].ToString()));
                }

                do
                {
                    var last = tweets.Last();
                    if (last == previous)
                        break;
                    else
                        previous = last;
                    _address = "https://api.twitter.com/1.1/search/tweets.json?q=czterydychylecha&count=100&max_id=" + last.post_id;
                    response = await client.GetAsync(_address);
                    if (response.IsSuccessStatusCode)
                    {
                        statuses = await response.Content.ReadAsAsync<JToken>();
                        foreach (var x in statuses["statuses"])
                        {
                            tweets.Add(new ReadTwitter(x["created_at"].ToString(), x["id_str"].ToString(), x["text"].ToString(), x["user"]["id_str"].ToString(), x["user"]["name"].ToString(), x["user"]["screen_name"].ToString()));
                        }
                    }
                } while (true);
                return tweets;
            }
            return null;
        }
    }
}