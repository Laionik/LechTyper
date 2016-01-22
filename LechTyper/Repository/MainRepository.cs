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

        public List<UserProfile> GetAllUsers()
        {
            return dbUser.UserProfiles.ToList();
        }

    }
}