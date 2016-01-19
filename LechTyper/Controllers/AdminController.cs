using LechTyper.Filters;
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
        MatchContext dbMatch = new MatchContext();
        TwitterContext dbTwitt = new TwitterContext();

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
            var UsersList = dbUser.UserProfiles.ToList().OrderBy(c => c.UserId);
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
                    return RedirectToAction("DatabaseError", "Error", e.Message);
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
                return RedirectToAction("DatabaseError", "Error", e.Message);
            }

            return RedirectToAction("UserIndex", new { page = page });
        }
        #endregion

        #region MatchEdit
        [Authorize(Roles = "admin")]
        public ActionResult MatchIndex(int? page)
        {
            ViewBag.UserManage = "Zarządzanie meczami";
            var MatchesList = dbMatch.MatchData.ToList();
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(MatchesList.ToPagedList(currentPageIndex, 20));
        }

        [Authorize(Roles = "admin")]
        public ActionResult MatchEdit()
        {
            int id = int.Parse(Request.QueryString["x"]);
            var page = Request.QueryString["page"];
            var MatchesList = dbMatch.MatchData.ToList();
            ViewBag.Page = page;
            var match = MatchesList.Find(r => r.id == id);
            return View(match);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult MatchEdit(string matchid, string date, string Competition, string host, string guest, string FTHostGoal, string FTGuestGoal, string isCompleted)
        {
            var match = dbMatch.MatchData.ToList();
            var up = match.Find(a => a.id == int.Parse(matchid));
            up.id = int.Parse(matchid);
            up.matchDate = DateTime.Parse(date);
            up.competition = Competition;
            up.host = host;
            up.guest = guest;
            up.finalHostGoal = int.Parse(FTHostGoal);
            up.finalGuestGoal = int.Parse(FTGuestGoal);
            up.isCompleted = bool.Parse(isCompleted);
            if (TryUpdateModel(up))
            {
                try
                {
                    dbMatch.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = e;
                    return RedirectToAction("DatabaseError", "Error", e.Message);
                }
            }
            return RedirectToAction("MatchIndex");
        }


        [Authorize(Roles = "admin")]
        public ActionResult MatchDelete()
        {
            var id = Request.QueryString["x"];
            var page = Request.QueryString["page"];
            var x = dbMatch.MatchData.ToList();
            var up = x.Find(a => a.id == int.Parse(id));
            try
            {
                dbMatch.MatchData.Remove(up);
                dbMatch.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e;
                return RedirectToAction("DatabaseError", "Error", e.Message);
            }

            return RedirectToAction("MatchIndex", new { page = page });
        }
        #endregion

        #region TwittEdit
        [Authorize(Roles = "admin")]
        public ActionResult TwittIndex(int? page)
        {
            ViewBag.UserManage = "Zarządzanie typami";
            var TwittList = dbTwitt.Tweets.ToList().OrderBy(c => c.Id);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(TwittList.ToPagedList(currentPageIndex, 20));
        }

        [Authorize(Roles = "admin")]
        public ActionResult TwittEdit()
        {
            int id = int.Parse(Request.QueryString["x"]);
            var page = Request.QueryString["page"];
            var TwittList = dbTwitt.Tweets.ToList();
            ViewBag.Page = page;
            var Twitt = TwittList.Find(r => r.Id == id);
            return View(Twitt);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult TwittEdit(string Twittid, string created_at, string post_id, string text, string user_id, string user_name, string user_nick)
        {
            //var x = dbTwitt.Tweets.ToList();
            //var up = x.Find(a => a.Twittid == int.Parse(Twittid));
            //up.Twittid = int.Parse(Twittid);
            //up.created_at = created_at;
            //up.post_id = post_id;
            //up.user_id = user_id;
            //up.user_name = user_name;
            //up.user_nick = user_nick;
            //up.text = text;
            //if (TryUpdateModel(up))
            //{
            //    try
            //    {
            //        dbTwitt.SaveChanges();
            //    }
            //    catch (Exception e)
            //    {
            //        ViewBag.ErrorMessage = e;
            //        return RedirectToAction("DatabaseError", "Error", e.Message);
            //    }
            //}
            return RedirectToAction("TwittIndex");
        }


        [Authorize(Roles = "admin")]
        public ActionResult TwittDelete()
        {
            var id = Request.QueryString["x"];
            var page = Request.QueryString["page"];
            var TwittList = dbTwitt.Tweets.ToList();
            var up = TwittList.Find(a => a.Id == int.Parse(id));
            try
            {
                dbTwitt.Tweets.Remove(up);
                dbTwitt.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e;
                return RedirectToAction("DatabaseError", "Error", e.Message);
            }

            return RedirectToAction("TwittIndex", new { page = page });
        }
        #endregion
    }
}