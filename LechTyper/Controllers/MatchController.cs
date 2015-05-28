using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using LechTyper.Models;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Net.Http;
using LechTyper.OAuth;
using System.Net;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;


namespace LechTyper.Controllers
{
    public class MatchController : Controller
    {
        public string dateParse(string toParse)
        {
            return Regex.Replace(toParse, @"\s*\([0-9]+\)\s*", "");
        }

        public void ResultParse(string result, out string goal1, out string goal2)
        {
            var temp = result.Replace("-", " ").Split(' ');
            goal1 = temp[0];
            goal2 = temp[1];
        }

        public List<Game> gamesParse(List<string> games, string comp)
        {
            Regex goalrgx = new Regex(@"^[0-9]+$");
            Regex clubrgx = new Regex(@"Lech Pozna�");
            string goal1, goal2;
            List<Game> returnGame = new List<Game>();
            for (int i = 0; i < games.Count(); i += 4)
            {
                if (clubrgx.IsMatch(games[i]) || clubrgx.IsMatch(games[i + 2]))
                {
                    ResultParse(games[i + 1], out goal1, out goal2);
                    if (goalrgx.IsMatch(goal1) && goalrgx.IsMatch(goal2))
                        returnGame.Add(new Game(dateParse(games[i + 3]), comp, games[i], games[i + 2], int.Parse(goal1), int.Parse(goal2)));
                    else
                        returnGame.Add(new Game(dateParse(games[i + 3]), comp, games[i], games[i + 2]));
                }
            }
            return returnGame;
        }

        // GET: /FixtureTME/
        public ActionResult FixtureTME()
        {
            var htmlWeb = new HtmlWeb();
            var document = htmlWeb.Load("http://www.90minut.pl/liga/0/liga7466.html");
            var tableTag = document.DocumentNode.SelectNodes("//table[@class='main']");
            HtmlAgilityPack.HtmlNode temp = tableTag.FirstOrDefault();
            var trTags = from x in temp.SelectNodes("//tr[@align='left']/td[@valign='top']")
                         where x != null
                         select x.InnerText;
            List<Game> fixture = gamesParse(trTags.ToList(), "Ekstraklasa");

            return View();
        }

        // GET: /FixturePP/
        public ActionResult FixturePP()
        {
            var htmlWeb = new HtmlWeb();
            var document = htmlWeb.Load("http://www.90minut.pl/liga/0/liga7470.html");
            var tableTag = document.DocumentNode.SelectNodes("//table[@class='main']");
            HtmlAgilityPack.HtmlNode temp = tableTag.FirstOrDefault();
            var trTags = from x in temp.SelectNodes("//tr[@align='left']/td[@valign='top']")
                         where x != null
                         select x.InnerText;
            List<Game> fixture = gamesParse(trTags.ToList(), "Puchar Polski");

            return View();
        }




    }
}
