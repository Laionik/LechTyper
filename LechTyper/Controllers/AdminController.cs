﻿using LechTyper.Filters;
using LechTyper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcPaging;

namespace LechTyper.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AdminController : Controller
    {
        UsersContext dbUser = new UsersContext();
        GameContext dbGame = new GameContext();
        TwitterContext dbTweet = new TwitterContext();
        OddContext dbOdd = new OddContext();


        //
        // GET: /Admin/
        [Authorize(Roles = "admin")]
        public ActionResult Admin()
        {
            return View();
        }

        #region RoleEdit
        [Authorize(Roles = "admin")]
        public ActionResult RoleCreate()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleCreate(string RoleName)
        {
            Roles.CreateRole(Request.Form["RoleName"]);
            ViewBag.ResultMessage = "Rula utworzona!";
            return RedirectToAction("RoleIndex", "Admin");
        }

        [Authorize(Roles = "admin")]
        public ActionResult RoleIndex()
        {
            var roles = Roles.GetAllRoles();
            return View(roles);
        }

        [Authorize(Roles = "admin")]
        public ActionResult RoleDelete(string RoleName)
        {
            try
            {
                Roles.DeleteRole(RoleName);
                ViewBag.ResultMessage = "Rola usunięta!";
                return RedirectToAction("RoleIndex", "Admin");
            }
            catch (Exception)
            {
                ViewBag.ResultMessage = "Rola jest przypisana do użytkownika/ów!";
                return RedirectToAction("RoleIndex", "Admin");
            }

        }

        /// <summary>
        /// Create a new role to the user
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult RoleAddToUser()
        {
            SelectList list = new SelectList(Roles.GetAllRoles());
            ViewBag.Roles = list;

            return View();
        }

        /// <summary>
        /// Add role to the user
        /// </summary>
        /// <param name="RoleName"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string RoleName, string UserName)
        {

            if (Roles.IsUserInRole(UserName, RoleName))
            {
                ViewBag.ResultMessage = "Użytkownik ma już przypisaną rolę!";
            }
            else
            {
                Roles.AddUserToRole(UserName, RoleName);
                ViewBag.ResultMessage = "Użytkownik został przypisany do roli";
            }

            SelectList list = new SelectList(Roles.GetAllRoles());
            ViewBag.Roles = list;
            return View();
        }

        /// <summary>
        /// Get all the roles for a particular user
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                ViewBag.RolesForThisUser = Roles.GetRolesForUser(UserName);
                SelectList list = new SelectList(Roles.GetAllRoles());
                ViewBag.Roles = list;
            }
            return View("RoleAddToUser");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {

            if (Roles.IsUserInRole(UserName, RoleName))
            {
                Roles.RemoveUserFromRole(UserName, RoleName);
                ViewBag.ResultMessage = "Udało się usunąć rolę użytkownika!";
            }
            else
            {
                ViewBag.ResultMessage = "Użytkownik nie miał przypisanej tej roli.";
            }
            ViewBag.RolesForThisUser = Roles.GetRolesForUser(UserName);
            SelectList list = new SelectList(Roles.GetAllRoles());
            ViewBag.Roles = list;


            return View("RoleAddToUser");
        }
        #endregion

        #region UserEdit
        [Authorize(Roles = "admin")]
        public ActionResult UserIndex(int? page)
        {
            ViewBag.UserManage = "Zarządzanie użytkownikami";
            var UsersList = dbUser.UserProfiles.ToList();
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(UsersList.ToPagedList(currentPageIndex, 20));
        }

        [Authorize(Roles = "admin")]
        public ActionResult UserEdit()
        {
            int id = int.Parse(Request.QueryString["x"]);
            var page = Request.QueryString["page"];
            var Userslist = dbUser.UserProfiles.ToList();
            ViewBag.Page = page;
            var x = Userslist.Find(r => r.UserId == id);
            return View(x);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult UserEdit(string userid, string username, string usermail)
        {
            var x = dbUser.UserProfiles.ToList();
            var up = x.Find(a => a.UserId == int.Parse(userid));
            up.UserId = int.Parse(userid);
            up.UserName = username;
            up.UserMail = usermail;
            if (TryUpdateModel(up))
            {
                try
                {
                    dbUser.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = e;
                    return RedirectToAction("DatabaseError", "Error");
                }
            }
            return RedirectToAction("UserIndex");
        }


        [Authorize(Roles = "admin")]
        public ActionResult UserDelete()
        {
            var id = Request.QueryString["x"];
            var page = Request.QueryString["page"];
            var x = dbUser.UserProfiles.ToList();
            var up = x.Find(a => a.UserId == int.Parse(id));
            foreach (var role in Roles.GetRolesForUser(up.UserName))
                Roles.RemoveUserFromRole(up.UserName, role);
            try
            {
                dbUser.UserProfiles.Remove(up);
                dbUser.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e;
                return RedirectToAction("DatabaseError", "Error");
            }

            return RedirectToAction("UserIndex", new { page = page });
        }
        #endregion

        #region MatchEdit
        [Authorize(Roles = "admin")]
        public ActionResult MatchIndex(int? page)
        {
            ViewBag.UserManage = "Zarządzanie meczami";
            var MatchesList = dbGame.GameData.ToList();
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(MatchesList.ToPagedList(currentPageIndex, 20));
        }

        [Authorize(Roles = "admin")]
        public ActionResult MatchEdit()
        {
            int id = int.Parse(Request.QueryString["x"]);
            var page = Request.QueryString["page"];
            var MatchesList = dbGame.GameData.ToList();
            ViewBag.Page = page;
            var game = MatchesList.Find(r => r.MatchID == id);
            return View(game);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult MatchEdit(string matchid, string date, string Competition, string host, string guest, string FTHostGoal, string FTGuestGoal, string isCompleted)
        {
            var game = dbGame.GameData.ToList();
            var up = game.Find(a => a.MatchID == int.Parse(matchid));
            up.MatchID = int.Parse(matchid);
            up.date = date;
            up.Competition = Competition;
            up.Host = host;
            up.Guest = guest;
            up.FTHostGoal = int.Parse(FTHostGoal);
            up.FTGuestGoal = int.Parse(FTGuestGoal);
            up.isCompleted = bool.Parse(isCompleted);
            if (TryUpdateModel(up))
            {
                try
                {
                    dbGame.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = e;
                    return RedirectToAction("DatabaseError", "Error");
                }
            }
            return RedirectToAction("MatchIndex");
        }


        [Authorize(Roles = "admin")]
        public ActionResult MatchDelete()
        {
            var id = Request.QueryString["x"];
            var page = Request.QueryString["page"];
            var x = dbGame.GameData.ToList();
            var up = x.Find(a => a.MatchID == int.Parse(id));
            try
            {
                dbGame.GameData.Remove(up);
                dbGame.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e;
                return RedirectToAction("DatabaseError", "Error");
            }

            return RedirectToAction("MatchIndex", new { page = page });
        }
        #endregion

        #region TweetEdit
        [Authorize(Roles = "admin")]
        public ActionResult TweetIndex(int? page)
        {
            ViewBag.UserManage = "Zarządzanie typami";
            var TweetsList = dbTweet.Tweets.ToList();
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(TweetsList.ToPagedList(currentPageIndex, 20));
        }

        [Authorize(Roles = "admin")]
        public ActionResult TweetEdit()
        {
            int id = int.Parse(Request.QueryString["x"]);
            var page = Request.QueryString["page"];
            var TweetsList = dbTweet.Tweets.ToList();
            ViewBag.Page = page;
            var tweet = TweetsList.Find(r => r.tweetid == id);
            return View(tweet);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult TweetEdit(string tweetid, string created_at, string post_id, string text, string user_id, string user_name, string user_nick)
        {
            var x = dbTweet.Tweets.ToList();
            var up = x.Find(a => a.tweetid == int.Parse(tweetid));
            up.tweetid = int.Parse(tweetid);
            up.created_at = created_at;
            up.post_id = post_id;
            up.user_id = user_id;
            up.user_name = user_name;
            up.user_nick = user_nick;
            up.text = text;
            if (TryUpdateModel(up))
            {
                try
                {
                    dbTweet.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = e;
                    return RedirectToAction("DatabaseError", "Error");
                }
            }
            return RedirectToAction("TweetIndex");
        }


        [Authorize(Roles = "admin")]
        public ActionResult TweetDelete()
        {
            var id = Request.QueryString["x"];
            var page = Request.QueryString["page"];
            var TweetsList = dbTweet.Tweets.ToList();
            var up = TweetsList.Find(a => a.tweetid == int.Parse(id));
            try
            {
                dbTweet.Tweets.Remove(up);
                dbTweet.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e;
                return RedirectToAction("DatabaseError", "Error");
            }

            return RedirectToAction("TweetIndex", new { page = page });
        }
        #endregion

        #region OddEdit
        [Authorize(Roles = "admin")]
        public ActionResult OddIndex(int? page)
        {
            ViewBag.UserManage = "Zarządzanie kursami";
            var OddsList = dbOdd.Odds.ToList();
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(OddsList.ToPagedList(currentPageIndex, 20));
        }

        [Authorize(Roles = "admin")]
        public ActionResult OddEdit()
        {
            int id = int.Parse(Request.QueryString["x"]);
            var page = Request.QueryString["page"];
            var OddsList = dbOdd.Odds.ToList();
            ViewBag.Page = page;
            var tweet = OddsList.Find(r => r.OddID == id);
            return View(tweet);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult OddEdit(string OddID, string MatchID, string odd_1, string odd_x, string odd_2)
        {
            var x = dbOdd.Odds.ToList();
            var up = x.Find(a => a.OddID == int.Parse(OddID));
            up.MatchID = int.Parse(MatchID);
            up.odd_1 = int.Parse(odd_1);
            up.odd_x = int.Parse(odd_x);
            up.odd_2 = int.Parse(odd_2);
            if (TryUpdateModel(up))
            {
                try
                {
                    dbOdd.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = e;
                    return RedirectToAction("DatabaseError", "Error");
                }
            }
            return RedirectToAction("OddIndex");
        }

        [Authorize(Roles = "admin")]
        public ActionResult OddCreate()
        {
            var MatchList = dbGame.GameData.ToList();
            var OddsList = dbOdd.Odds.ToList();
            Odd odd = new Odd();
            ViewBag.Games = MatchList;
            ViewBag.Odds = OddsList;
            return View(odd);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult OddCreate(string OddID, string MatchID, string odd_1, string odd_x, string odd_2)
        {
            var oddslist = dbOdd.Odds.ToList();
            var odd = oddslist.Find(a => a.MatchID == int.Parse(MatchID));
            if (odd == null)
            {
                    try
                    {
                        dbOdd.Odds.Add(new Odd(int.Parse(MatchID), double.Parse(odd_1), double.Parse(odd_x), double.Parse(odd_2)));
                        dbOdd.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = e;
                        return RedirectToAction("DatabaseError", "Error");
                    }
                    ViewBag.OddCreated = "Udało się utworzyć nowy kurs";
            }
            return RedirectToAction("OddIndex");
        }


        [Authorize(Roles = "admin")]
        public ActionResult OddDelete()
        {
            var id = Request.QueryString["x"];
            var page = Request.QueryString["page"];
            var OddsList = dbOdd.Odds.ToList();
            var up = OddsList.Find(a => a.OddID == int.Parse(id));
            try
            {
                dbOdd.Odds.Remove(up);
                dbOdd.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e;
                return RedirectToAction("DatabaseError", "Error");
            }

            return RedirectToAction("OddIndex", new { page = page });
        }
        #endregion
    }
}