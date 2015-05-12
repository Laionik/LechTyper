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

namespace LechTyper.Controllers
{
    public class TwitterController : Controller
    {
        //
        // GET: /Twitter/

        public async Task<ActionResult> Twitter()
        {
            JToken tweets = await RunClient();
            var results = tweets["statuses"].Children().ToList();
            if (tweets != null)
            {
                return View(results);
            }
            else
            {
                return View();
            }
        }


        async Task<JToken> RunClient()
        {
            string _address = "https://api.twitter.com/1.1/search/tweets.json?q=RAZEMpoMistrzostwo&count=100";
            HttpClient client = new HttpClient(new OAuthMessageHandler(new HttpClientHandler()));
            HttpResponseMessage response = await client.GetAsync(_address);
            if (response.IsSuccessStatusCode)
            {
                JToken statuses = await response.Content.ReadAsAsync<JToken>();

                do
                {
                    var child = statuses.Last();
                    _address = "https://api.twitter.com/1.1/search/tweets.json?q=RAZEMpoMistrzostwo&count=100&max_id=" + (int.Parse(child["id_str"].ToString()) - 1);
                    response = await client.GetAsync(_address);
                    if (response.IsSuccessStatusCode)
                    {
                        statuses.AddAfterSelf(await response.Content.ReadAsAsync<JToken>());
                    }
                } while (statuses.Count() % 100 == 0);
                return statuses;
            }
            return null;
        }
    }
}