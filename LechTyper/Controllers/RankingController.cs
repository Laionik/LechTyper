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
    public class RankingController : Controller
    {
        RankingContext dbRank = new RankingContext();
        TwitterContext dbTwitt = new TwitterContext();

        // GET: /Ranking/

        public ActionResult RankingTable()
        {
            ViewBag.Title = "Tabela bukmacherska";
            ViewBag.Twitter = dbTwitt.Twitts.ToList();
            return View(dbRank.Ranks.ToList().OrderByDescending(C=> C.Balance).ThenByDescending(C=> C.Played_number).ThenByDescending(C=> C.UserID));
        }

    }
}
