﻿using System;
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
using System.Data.Objects;


namespace LechTyper.Controllers
{
    public class MatchController : Controller
    {
        MatchContext dbMatch = new MatchContext();
        Dictionary<string, string> dateParser = new Dictionary<string, string>();

        private string ekstraklasaUrl = "http://www.90minut.pl/liga/0/liga8069.html";
        private string polishCupUrl = "http://www.90minut.pl/liga/0/liga8073.html";
        private string supercupUrl = "http://www.90minut.pl/liga/0/liga8072.html";
        private string championsLeagueUrl = "http://www.90minut.pl/liga/0/liga8092.html";
        private string europaLeagueUrl = "http://www.90minut.pl/liga/0/liga8093.html";

        public MatchController()
        {
            dateParser.Add("stycznia", "01");
            dateParser.Add("lutego", "02");
            dateParser.Add("marca", "03");
            dateParser.Add("kwietnia", "04");
            dateParser.Add("maja", "05");
            dateParser.Add("czerwca", "06");
            dateParser.Add("lipca", "07");
            dateParser.Add("sierpnia", "08");
            dateParser.Add("wrzesnia", "09");
            dateParser.Add("pazdziernika", "10");
            dateParser.Add("listopada", "11");
            dateParser.Add("grudnia", "12");


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


        /// <summary>
        /// Przetwarzanie daty
        /// </summary>
        /// <param name="toParse">Data w formacie "18 lipca"</param>
        /// <returns>Data</returns>
        public DateTime dateParse(string toParse)
        {
            if (toParse == "")
                return DateTime.Parse("2000.01.01");
            else
            {
                var tempDate = System.Text.RegularExpressions.Regex.Replace(toParse, @",\s[0-9]{2}:[0-9]{2}\s*(\([0-9]+\s*[0-9]+\)\s*)?", "");
                tempDate = RemovePolishLetters(tempDate);
                var tempMonth = dateParser[tempDate.Split(' ')[1]];
                if (int.Parse(tempMonth) >= 7)
                    tempMonth = (DateTime.Now.Year - 1).ToString() + "." + tempMonth;
                else
                    tempMonth = DateTime.Now.Year.ToString() + "." + tempMonth;
                tempDate = System.Text.RegularExpressions.Regex.Replace(tempDate, @"\s*[a-z]+\s*", "");
                return DateTime.Parse(tempMonth + "." + System.Text.RegularExpressions.Regex.Replace(tempDate, @"\s*[a-z]+\s*", ""));
            }

        }
        /// <summary>
        /// Pobieranie liczby bramek z framentu tekstu
        /// </summary>
        /// <param name="result">Tekst wyniku</param>
        /// <param name="goal1">Liczba goli gospodarza</param>
        /// <param name="goal2">Liczba goli gości</param>
        public void ResultParse(string result, out string goal1, out string goal2)
        {
            var temp = result.Replace("-", " ").Split(' ');
            goal1 = temp[0];
            goal2 = temp[1].Replace("\n", "");
        }
        /// <summary>
        /// Usuwanie białych znaków
        /// </summary>
        /// <param name="text">Tekst do przetworzenia</param>
        /// <returns>Poprawny string</returns>
        public string RemoveWhiteSpaces(string text)
        {
            text = System.Text.RegularExpressions.Regex.Replace(text, @"^\s*", "");
            return System.Text.RegularExpressions.Regex.Replace(text, @"\s*$", "").Replace("&quot;", "\"");
        }

        /// <summary>
        /// Parsowanie strony www
        /// </summary>
        /// <param name="wwwToParse">Lista wartości strony WWW</param>
        /// <param name="comp">Typ rozgrywek</param>
        /// <returns>Lista meczów danych rozgrywek</returns>
        public List<Match> MatchParse(List<string> wwwToParse, string comp)
        {
            System.Text.RegularExpressions.Regex goalrgx = new System.Text.RegularExpressions.Regex(@"^[0-9]+$");
            System.Text.RegularExpressions.Regex clubrgx = new System.Text.RegularExpressions.Regex(@"Lech Poznań");
            string goal1, goal2;
            int matchDiff = 0, dateDiff = 0;
            if (comp == "ChampionsLeague" || comp == "EuropaLeague")
            {
                matchDiff = 1;
                dateDiff = 2;
            }

            List<Match> returnMatch = new List<Match>();
            for (int i = 0; i < wwwToParse.Count(); i += 4 + dateDiff)
            {
                if (clubrgx.IsMatch(wwwToParse[i + matchDiff]) || clubrgx.IsMatch(wwwToParse[i + 2 + matchDiff]))
                {
                    ResultParse(wwwToParse[i + 1 + matchDiff], out goal1, out goal2);
                    if (goalrgx.IsMatch(goal1) && goalrgx.IsMatch(goal2))
                        returnMatch.Add(new Match(dateParse(wwwToParse[i + 3 + dateDiff]), comp, RemoveWhiteSpaces(wwwToParse[i + matchDiff]), RemoveWhiteSpaces(wwwToParse[i + 2 + matchDiff]), int.Parse(goal1), int.Parse(goal2), true));
                    else
                        returnMatch.Add(new Match(dateParse(wwwToParse[i + 3 + dateDiff]), comp, RemoveWhiteSpaces(wwwToParse[i + matchDiff]), RemoveWhiteSpaces(wwwToParse[i + 2 + matchDiff]), false));
                }
            }
            return returnMatch;
        }

        /// <summary>
        /// Aktualizacja rozgrywek
        /// </summary>
        /// <param name="competition">Nazwa rozgrywek</param>
        /// <returns>Widok</returns>
        [Authorize(Roles = "admin")]
        public ActionResult UpdateCompetition(string competition)
        {
            Encoding encoding = Encoding.GetEncoding("iso-8859-2");
            var htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = encoding,
            };
            string url = "";
            switch (competition)
            {
                case "Ekstraklasa":
                    url = ekstraklasaUrl;
                    break;
                case "PolishCup":
                    url = polishCupUrl;
                    break;
                case "Supercup":
                    url = supercupUrl;
                    break;
                case "ChampionsLeague":
                    url = championsLeagueUrl;
                    break;
                case "EuropaLeague":
                    url = europaLeagueUrl;
                    break;
            }

            var document = htmlWeb.Load(url);
            var wwwToParse = document.DocumentNode.SelectNodes("//table[@class='main']").FirstOrDefault().SelectNodes("//tr[@align='left']/td[@valign='top']").Where(x => x != null).Select(x => x.InnerText).ToList();
            List<HtmlNode> test = new List<HtmlNode>();

            List<Match> matchList = MatchParse(wwwToParse, competition);
            try
            {
                foreach (var match in matchList)
                {
                    var defaultDate = DateTime.Parse("2000-01-01");
                    Match matchModel = dbMatch.MatchData.FirstOrDefault(u => (EntityFunctions.TruncateTime(u.date) == EntityFunctions.TruncateTime(match.date) || EntityFunctions.TruncateTime(u.date) == EntityFunctions.TruncateTime(defaultDate)) && u.host == match.host && u.guest == match.guest);
                    try
                    {
                        if (matchModel == null)
                        {
                            dbMatch.MatchData.Add(match);
                        }
                        else if (matchModel.isCompleted != match.isCompleted)
                        {
                            matchModel.date = match.date;
                            matchModel.finalGuestGoal = match.finalGuestGoal;
                            matchModel.finalHostGoal = match.finalHostGoal;
                            matchModel.isCompleted = true;
                            UpdateModel(matchModel);
                        }
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("Error", "Error", e.Message);
                    }
                }
                dbMatch.SaveChanges();
            }
            catch (Exception e)
            {
                return RedirectToAction("DatabaseError", "Error", e.Message);
            }
            return View(matchList);
        }

        /// <summary>
        /// Wyświetlanie meczów danych rozgrywek
        /// </summary>
        /// <returns>Widok</returns>
        [AllowAnonymous]
        public ActionResult MatchDisplay(string competition)
        {
            switch (competition)
            {
                case "Ekstraklasa":
                    ViewBag.title = Resources.ResourceMain.Ekstraklasa;
                    break;
                case "PolishCup":
                    ViewBag.title = Resources.ResourceMain.PolishCup;
                    break;
                case "Supercup":
                    ViewBag.title = Resources.ResourceMain.PolishSupercup;
                    break;
                case "ChampionsLeague":
                    ViewBag.title = Resources.ResourceMain.ChampionsLeague;
                    break;
                case "EuropaLeague":
                    ViewBag.title = Resources.ResourceMain.EuropaLeague;
                    break;
            }
            return View(dbMatch.MatchData.Where(x => x.competition == competition).ToList());
        }
    }
}