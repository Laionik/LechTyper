using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using LechTyper.Models;
using HtmlAgilityPack;
using System.Net.Http;
using LechTyper.OAuth;
using System.Net;
using System.Text.RegularExpressions;

namespace LechTyper.Controllers
{
    public class OddController : Controller
    {
        //
        // GET: /Odd/
        OddContext dbOdd = new OddContext();
        TagContext dbTag = new TagContext();
        TwitterContext dbTwitt = new TwitterContext();
        GameContext dbGame = new GameContext();
        public ActionResult Odd()
        {
            ViewBag.Title = "Typy";
            //Encoding iso = Encoding.GetEncoding("iso-8859-2");
            //var htmlWeb = new HtmlWeb()
            //{
            //    AutoDetectEncoding = false,
            //    OverrideEncoding = iso,
            //};
            //var document = htmlWeb.Load("http://www.eurotip.pl/index.php?cmd=offer-full&category_id=1&event_id=136");
            //var tableTag = document.DocumentNode.SelectNodes("//table[@class='main']");
            //HtmlAgilityPack.HtmlNode temp = tableTag.FirstOrDefault();
            //var trTags = from x in temp.SelectNodes("//tr[@align='left']/td[@valign='top']")
            //             where x != null
            //             select x.InnerText;
            //List<Game> fixture = gamesParse(trTags.ToList(), "Superpuchar");

            return View();
        }

        public ActionResult BetResult()
        {
            ViewBag.Title = "Wyniki typowania";
            //posortuj listę po dacie, wbyierz mniejsze od dzisiaj i wybierz ostatni - lub ustaw Task na dzień po i wybierz twitty na dzień przed - wtedy może obejdzie się bez TagModels
            var TagsList = dbTag.Tags.ToList();
            var odd = dbOdd.Odds.ToList().Find(x => x.MatchID == TagsList[0].MatchID);
            var game = dbGame.GameData.ToList().Find(x => x.MatchID == TagsList[0].MatchID);
            var BetsList = dbTwitt.Tweets.ToList();
            string tag = "#" + TagsList[0].TagText;
            Regex tagrgx = new Regex(tag);
            string result = game.FTHostGoal + ":" + game.FTGuestGoal;
            int hda = (game.FTHostGoal > game.FTGuestGoal) ? 1 : (game.FTHostGoal == game.FTGuestGoal) ? 0 : 2;
            List<Tweet> won = new List<Tweet>(); //exactly result
            List<Tweet> won_hda = new List<Tweet>(); //homde/draw/away
            foreach (var bet in BetsList)
            {
                int bet_hda = (bet.text[0] > bet.text[2]) ? 1 : (bet.text[0] > bet.text[2]) ? 0 : 2;
                if(tagrgx.IsMatch(bet.text))
                {
                    if(Regex.Replace(bet.text, @"\s*" + tag + @".*", "") == result)
                    {
                        won.Add(bet);
                    }
                    else if (bet_hda == hda)
                    {
                        won_hda.Add(bet);
                    }
                }
            }
            ViewBag.hda = won_hda;
            return View(won);
        }
    }
}
