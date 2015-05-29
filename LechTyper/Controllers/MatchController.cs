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
        GameContext db = new GameContext();

        #region GetResults
        public string dateParse(string toParse)
        {
            return Regex.Replace(toParse, @"\s*\([0-9]+\s*[0-9]+\)\s*", "");
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
            Regex clubrgx = new Regex(@"Lech Poznań");
            string goal1, goal2;
            List<Game> returnGame = new List<Game>();
            for (int i = 0; i < games.Count(); i += 4)
            {
                if (clubrgx.IsMatch(games[i]) || clubrgx.IsMatch(games[i + 2]))
                {
                    ResultParse(games[i + 1], out goal1, out goal2);
                    if (goalrgx.IsMatch(goal1) && goalrgx.IsMatch(goal2))
                        returnGame.Add(new Game(dateParse(games[i + 3]), comp, games[i].Replace("\n", ""), games[i + 2].Replace("\n", ""), int.Parse(goal1), int.Parse(goal2), true));
                    else
                        returnGame.Add(new Game(dateParse(games[i + 3]), comp, games[i].Replace("\n", ""), games[i + 2].Replace("\n", ""), false));
                }
            }
            return returnGame;
        }

        public List<Game> gamesParseINT(List<string> games, string comp)
        {
            Regex goalrgx = new Regex(@"^[0-9]+$");
            Regex clubrgx = new Regex(@"Lech Poznań");
            string goal1, goal2;
            List<Game> returnGame = new List<Game>();
            for (int i = 0; i < games.Count(); i += 6)
            {
                if (clubrgx.IsMatch(games[i + 1]) || clubrgx.IsMatch(games[i + 3]))
                {
                    ResultParse(games[i + 2], out goal1, out goal2);
                    if (goalrgx.IsMatch(goal1) && goalrgx.IsMatch(goal2))
                        returnGame.Add(new Game(dateParse(games[i + 5]), comp, games[i + 1].Replace("\n", ""), games[i + 3].Replace("\n", ""), int.Parse(goal1), int.Parse(goal2), true));
                    else
                        returnGame.Add(new Game(dateParse(games[i + 5]), comp, games[i + 1].Replace("\n", ""), games[i + 3].Replace("\n", ""), false));
                }
            }
            return returnGame;
        }
        #endregion

        // GET: /FixtureTME/
        public ActionResult FixtureTME()
        {
            Encoding iso = Encoding.GetEncoding("iso-8859-2");
            var htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = iso,
            };
            var document = htmlWeb.Load("http://www.90minut.pl/liga/0/liga7466.html");
            var tableTag = document.DocumentNode.SelectNodes("//table[@class='main']");
            HtmlAgilityPack.HtmlNode temp = tableTag.FirstOrDefault();
            var trTags = from x in temp.SelectNodes("//tr[@align='left']/td[@valign='top']")
                         where x != null
                         select x.InnerText;
            List<Game> fixture = gamesParse(trTags.ToList(), "Ekstraklasa");
            foreach (var x in fixture)
            {
                Game matchdata = db.GameData.FirstOrDefault(u => u.date.ToLower() == x.date.ToLower());
                try
                {
                    if (matchdata == null)
                    {
                        db.GameData.Add(x);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    return RedirectToAction("DatabaseError", "Error");
                }
            }
            return View(fixture);
        }

        // GET: /FixturePP/
        public ActionResult FixturePP()
        {

            Encoding iso = Encoding.GetEncoding("iso-8859-2");
            var htmlWeb = new HtmlWeb()
              {
                  AutoDetectEncoding = false,
                  OverrideEncoding = iso,
              };
            var document = htmlWeb.Load("http://www.90minut.pl/liga/0/liga7470.html");

            var tableTag = document.DocumentNode.SelectNodes("//table[@class='main']");
            HtmlAgilityPack.HtmlNode temp = tableTag.FirstOrDefault();
            var trTags = from x in temp.SelectNodes("//tr[@align='left']/td[@valign='top']")
                         where x != null
                         select x.InnerText;
            List<Game> fixture = gamesParse(trTags.ToList(), "Puchar Polski");
            foreach (var x in fixture)
            {
                Game matchdata = db.GameData.FirstOrDefault(u => u.date.ToLower() == x.date.ToLower());
                try
                {
                    if (matchdata == null)
                    {
                        db.GameData.Add(x);
                        db.SaveChanges();
                    }
                    else if (TryUpdateModel(x))
                    {
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            return RedirectToAction("DatabaseError", "Error");
                        }
                    }
                }
                catch (Exception e)
                {
                    return RedirectToAction("DatabaseError", "Error");
                }
            }
            return View(fixture);
        }

        // GET: /FixtureSP/
        public ActionResult FixtureSP()
        {
            Encoding iso = Encoding.GetEncoding("iso-8859-2");
            var htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = iso,
            };
            var document = htmlWeb.Load("http://www.90minut.pl/liga/0/liga7465.html");
            var tableTag = document.DocumentNode.SelectNodes("//table[@class='main']");
            HtmlAgilityPack.HtmlNode temp = tableTag.FirstOrDefault();
            var trTags = from x in temp.SelectNodes("//tr[@align='left']/td[@valign='top']")
                         where x != null
                         select x.InnerText;
            List<Game> fixture = gamesParse(trTags.ToList(), "Superpuchar");
            foreach (var x in fixture)
            {
                Game matchdata = db.GameData.FirstOrDefault(u => u.date.ToLower() == x.date.ToLower());
                try
                {
                    if (matchdata == null)
                    {
                        db.GameData.Add(x);
                        db.SaveChanges();
                    }
                    else if (TryUpdateModel(x))
                    {
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            return RedirectToAction("DatabaseError", "Error");
                        }
                    }
                }
                catch (Exception e)
                {
                    return RedirectToAction("DatabaseError", "Error");
                }
            }
            return View(fixture);
        }

        // GET: /FixtureLM/
        public ActionResult FixtureLM()
        {
            Encoding iso = Encoding.GetEncoding("iso-8859-2");
            var htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = iso,
            };
            var document = htmlWeb.Load("http://www.90minut.pl/liga/0/liga7474.html");
            var tableTag = document.DocumentNode.SelectNodes("//table[@class='main']");
            HtmlAgilityPack.HtmlNode temp = tableTag.FirstOrDefault();
            var trTags = from x in temp.SelectNodes("//tr[@align='left']/td[@valign='top']")
                         where x != null
                         select x.InnerText;
            List<Game> fixture = gamesParseINT(trTags.ToList(), "Liga Mistrzów");
            foreach (var x in fixture)
            {
                Game matchdata = db.GameData.FirstOrDefault(u => u.date.ToLower() == x.date.ToLower());
                try
                {
                    if (matchdata == null)
                    {
                        db.GameData.Add(x);
                        db.SaveChanges();
                    }
                    else if (TryUpdateModel(x))
                    {
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            return RedirectToAction("DatabaseError", "Error");
                        }
                    }
                }
                catch (Exception e)
                {
                    return RedirectToAction("DatabaseError", "Error");
                }
            }
            return View(fixture);
        }

        // GET: /FixtureLE/
        public ActionResult FixtureLE()
        {
            Encoding iso = Encoding.GetEncoding("iso-8859-2");
            var htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = iso,
            };
            var document = htmlWeb.Load("http://www.90minut.pl/liga/0/liga7475.html");
            var tableTag = document.DocumentNode.SelectNodes("//table[@class='main']");
            HtmlAgilityPack.HtmlNode temp = tableTag.FirstOrDefault();
            var trTags = from x in temp.SelectNodes("//tr[@align='left']/td[@valign='top']")
                         where x != null
                         select x.InnerText;
            List<Game> fixture = gamesParseINT(trTags.ToList(), "Liga Europy");
            foreach (var x in fixture)
            {
                Game matchdata = db.GameData.FirstOrDefault(u => u.date.ToLower() == x.date.ToLower());
                try
                {
                    if (matchdata == null)
                    {
                        db.GameData.Add(x);
                        db.SaveChanges();
                    }
                    else if (TryUpdateModel(x))
                    {
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            return RedirectToAction("DatabaseError", "Error");
                        }
                    }
                }
                catch (Exception e)
                {
                    return RedirectToAction("DatabaseError", "Error");
                }
            }
            return View(fixture);
        }
    }
}
