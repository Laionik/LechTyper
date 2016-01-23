using LechTyper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LechTyper.Repository
{
    public class MainRepository
    {
        private UsersContext dbUser;

        public MainRepository(UsersContext dbUser) 
        {
            this.dbUser = dbUser;
        }

        /// <summary>
        /// Pobieranie listy wszystkich zarejestrowanych użytkowników
        /// </summary>
        /// <returns>Lista wszystkich użytkowników</returns>

        public List<UserProfile> GetUsers()
        {
            return dbUser.UserProfiles.ToList();
        }
        /// <summary>
        /// Pobieranie listy loginów wszystkich zarejestrowanych użytkowników
        /// </summary>
        /// <returns>Lista loginów wszystkich użytkowników</returns>
        public List<string> GetUsersNames()
        {
            return dbUser.UserProfiles.Select(u => u.UserName).ToList();
        }

        /// <summary>
        /// Wyszukiwanie użytkownika po ID
        /// </summary>
        /// <returns>Użytkownik</returns>
        public UserProfile GetUserById(int id)
        {
            return dbUser.UserProfiles.Where(m => m.UserId == id).FirstOrDefault();
        }
    }
}