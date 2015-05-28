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
        // GET: /Fixture/

        //public async Task<ActionResult> Fixture()
        //{
        //    var fixture = await RunClient();
        //    if (fixture != null)
        //    {
        //        return View(fixture);
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}

        //static DateTime ConvertFromUnixTimestamp(double timestamp)
        //{
        //    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        //    return origin.AddSeconds(timestamp);
        //}
        //async Task<List<Game>> RunClient()
        //{
        //    string _address = "http://www.ekstraklasa.org/api/rozgrywki";
        //    HttpClient client = new HttpClient(new OAuthMessageHandler(new HttpClientHandler()));
        //    HttpResponseMessage response = await client.GetAsync(_address);
        //    List<Game> matches = new List<Game>();
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var jsonString = response.Content.ReadAsStringAsync().Result;
        //        JObject statuses = JObject.Parse(jsonString);
        //        foreach (var x in statuses["mecze"])
        //        {
        //            if (x["klub_a"].ToString() == "31839b036f63806cba3f47b93af8ccb5" || x["klub_b"].ToString() == "31839b036f63806cba3f47b93af8ccb5")
        //                if (x["status"].ToString() == "przed")
        //                    matches.Add(new Match("Ekstraklasa", x["klub_a_nazwa"].ToString(), x["klub_b_nazwa"].ToString(), int.Parse(x["kolejka"].ToString())));
        //            matches.Add(new Match("Ekstraklasa", x["klub_a_nazwa"].ToString(), x["klub_b_nazwa"].ToString(), int.Parse(x["kolejka"].ToString()), int.Parse(x["wynik_a"].ToString()), int.Parse(x["wynik_b"].ToString())));
        //        }
        //        matches = matches.OrderBy(o => o.MatchDay).ToList();
        //        return matches;
        //    }
        //    return null;
        //}

        //public string strReplace(string toReplace)
        //{
        //    toReplace = Regex.Replace(toReplace, @"\s+", " ");
        //    toReplace = Regex.Replace(toReplace, @"^\s+", "");
        //    toReplace = toReplace.Replace("\n", "");
        //    toReplace = toReplace.Replace(" - ", "-");
        //    toReplace = toReplace.Replace("Puchar Polski", "PP");
        //    toReplace = toReplace.Replace("Ekstraklasa", "EK");
        //    toReplace = toReplace.Replace("Liga Mistrzów UEFA", "LM");
        //    toReplace = toReplace.Replace("Liga Europy UEFA", "LE");
        //    toReplace = toReplace.Replace("Superpuchar", "SP");
        //    toReplace = toReplace.Replace("Club Friendlies", "TW");
        //    return toReplace;
        //}
        //public List<string> GamesList(List<string> sentence)
        //{
        //    Regex rgx = new Regex(@"^\s*[0-9]{2}/[0-9]{2}/[0-9]{4}\s+[1-9]?[0-9]:[0-9]{2}\s+[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]{2}\s+([a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+\s){1,3}([\?]|[0-9]{1,2})[-]([\?]|[0-9]{1,2})\s+([a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+\s){1,3}$");
        //    string x;
        //    List<string> output = new List<string>();
        //    foreach (string one in sentence)
        //    {
        //        x = strReplace(one);
        //        if (rgx.IsMatch(x))
        //        {
        //            output.Add(x);
        //        }
        //    }
        //    return output;
        //}

        //public List<Game> GamesParser(List<string> gameList)
        //{
        //    List<Game> temp = new List<Game>();
        //    foreach(string x in gameList)
        //    {
                
        //        //var gameTab = x.Split(' ');
        //        //temp.Add(new Game(DateTime.Parse(gameTab[0]+" "+gameTab[1]), gameTab[2], gameTab[3]+" "+gameTab[4], gameTab[6]+" "+gameTab[7],gameTab[5][0], gameTab[5][2]));
        //    }
        //    return temp;
        //}

        //public ActionResult FixtureHTML()
        //{
        //    var htmlWeb = new HtmlWeb();
        //    var document = htmlWeb.Load("http://www.meczyki.pl/druzyna,lech-poznan,1653");
        //    var TableTag = document.DocumentNode.SelectSingleNode("//table[@class='leaguetable teammatches']");

        //    var test = from x in TableTag.SelectNodes("//tr")
        //               where x != null
        //               select x.InnerText;
        //    List<string> gamesList = GamesList(test.ToList());

        //    //List<Game> MatchList = new List<Game>();
        //    //for (int i = 0; i < dayList.Count(); i ++)
        //    //{
        //    //    MatchList.Add(new Match(dayList.ToList()[i], team1List.ToList()[i], team2List.ToList()[i]));
        //    //}


        //    return View();
        //}

    }
}
