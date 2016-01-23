using LechTyper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LechTyper.Repository
{

    public class TwitterRepository
    { 
        private TwitterContext dbTwitter;

        public TwitterRepository(TwitterContext dbTwitter) 
        {
            this.dbTwitter = dbTwitter;
        }

        /// <summary>
        /// Najbliższa kolejka typerów
        /// </summary>
        /// <returns>Numer kolejki</returns>
        public List<Tweet> GetTweets()
        {
            return dbTwitter.Tweets.ToList();
        }

        /// <summary>
        /// Wyszukiwanie tweetu po ID
        /// </summary>
        /// <returns>Mecz</returns>
        public Tweet GetTweetById(int id)
        {
            return dbTwitter.Tweets.Where(m => m.Id == id).FirstOrDefault();
        }
    }
}