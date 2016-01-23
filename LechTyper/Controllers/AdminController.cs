using LechTyper.Filters;
using LechTyper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcPaging;
using LechTyper.Repository;

namespace LechTyper.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AdminController : Controller
    {
        UsersContext dbUser = new UsersContext();
        MatchContext dbMatch = new MatchContext();
        TwitterContext dbTwitt = new TwitterContext();
        private MainRepository _mainRepository;
        private MatchRepository _matchRepository;
        private TwitterRepository _twitterRepository;

        public AdminController()
        {
            _twitterRepository = new TwitterRepository(dbTwitt);
            _mainRepository = new MainRepository(dbUser);
            _matchRepository = new MatchRepository(dbMatch);
        }
        //
        // GET: /Admin/
        [Authorize(Roles = "admin")]
        public ActionResult Admin()
        {
            return View();
        }

        #region Role
        [Authorize(Roles = "admin")]
        public ActionResult RoleCreate()
        {
            return View();
        }
        /// <summary>
        /// Tworzenie roli
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Usuwanie roli
        /// </summary>
        /// <returns></returns>
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
            ViewBag.Roles = new SelectList(Roles.GetAllRoles());
            ViewBag.Users = new SelectList(_mainRepository.GetUsersNames());
            return View();
        }

        /// <summary>
        /// Dodaj użytkownikowi rolę
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
            ViewBag.Roles = new SelectList(Roles.GetAllRoles());
            ViewBag.Users = new SelectList(_mainRepository.GetUsersNames());
            return View();
        }

        /// <summary>
        /// Role dla danego użytkownika
        /// </summary>
        /// <param name="UserName">Nazwa użytkownika</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                ViewBag.RolesForThisUser = Roles.GetRolesForUser(UserName);

            ViewBag.Roles = new SelectList(Roles.GetAllRoles());
            ViewBag.Users = new SelectList(_mainRepository.GetUsersNames());
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
            ViewBag.Roles = new SelectList(Roles.GetAllRoles());
            ViewBag.Users = new SelectList(_mainRepository.GetUsersNames());

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
            int id = int.Parse(Request.QueryString["userId"]);
            var page = Request.QueryString["page"];
            ViewBag.Page = page;
            return View(_mainRepository.GetUserById(id));
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult UserEdit(string userid, string username, string usermail)
        {
            var user = _mainRepository.GetUserById(int.Parse(userid));
            user.UserName = username;
            user.UserMail = usermail;
            if (TryUpdateModel(user))
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
            var id = Request.QueryString["userId"];
            var page = Request.QueryString["page"];
            var user = _mainRepository.GetUserById(int.Parse(id));
            foreach (var role in Roles.GetRolesForUser(user.UserName))
                Roles.RemoveUserFromRole(user.UserName, role);
            try
            {
                dbUser.UserProfiles.Remove(user);
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
            int id = int.Parse(Request.QueryString["matchId"]);
            var page = Request.QueryString["page"];
            ViewBag.page = page;
            return View(_matchRepository.GetMatchById(id));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult MatchEdit(string matchid, string date, string Competition, string host, string guest, string finalHostGoal, string finalGuestGoal, string halfHostGoal, string halfGuestGoal, string isCompleted)
        {
            var match = _matchRepository.GetMatchById(int.Parse(matchid));
            match.date = DateTime.Parse(date);
            match.competition = Competition;
            match.host = host;
            match.guest = guest;
            match.finalHostGoal = int.Parse(finalHostGoal);
            match.finalGuestGoal = int.Parse(finalGuestGoal);
            match.halfHostGoal = int.Parse(halfHostGoal);
            match.halfGuestGoal = int.Parse(halfGuestGoal);
            match.isCompleted = bool.Parse(isCompleted);
            if (TryUpdateModel(match))
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
            var id = Request.QueryString["matchId"];
            var page = Request.QueryString["page"];
            var match = _matchRepository.GetMatchById(int.Parse(id));
            try
            {
                dbMatch.MatchData.Remove(match);
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

        #region TweetEdit
        [Authorize(Roles = "admin")]
        public ActionResult TweetIndex(int? page)
        {
            ViewBag.UserManage = "Zarządzanie typami";
            var TwittList = dbTwitt.Tweets.ToList().OrderBy(c => c.Id);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(TwittList.ToPagedList(currentPageIndex, 20));
        }

        [Authorize(Roles = "admin")]
        public ActionResult TweetEdit()
        {
            int id = int.Parse(Request.QueryString["tweetId"]);
            var page = Request.QueryString["page"];
            ViewBag.Page = page;
            var tweet = _twitterRepository.GetTweetById(id);
            return View(tweet);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult TweetEdit(string id, string result, string resultHalf, string scorer)
        {
            var tweet = _twitterRepository.GetTweetById(int.Parse(id));
            tweet.result = result;
            tweet.resultHalf = resultHalf;
            tweet.scorer = scorer;
            if (TryUpdateModel(tweet))
            {
                try
                {
                    dbTwitt.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = e;
                    return RedirectToAction("DatabaseError", "Error", e.Message);
                }
            }
            return RedirectToAction("TweetIndex");
        }


        [Authorize(Roles = "admin")]
        public ActionResult TwittDelete()
        {
            var id = Request.QueryString["tweetId"];
            var page = Request.QueryString["page"];
            var tweet = _twitterRepository.GetTweetById(int.Parse(id));
            try
            {
                dbTwitt.Tweets.Remove(tweet);
                dbTwitt.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e;
                return RedirectToAction("DatabaseError", "Error", e.Message);
            }

            return RedirectToAction("TweetIndex", new { page = page });
        }
        #endregion
    }
}