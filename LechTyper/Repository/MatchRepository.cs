using LechTyper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LechTyper.Repository
{
    public class MatchRepository
    {
         private MatchContext dbMatch;

         public MatchRepository(MatchContext dbMatch) 
        {
            this.dbMatch = dbMatch;
        }

        /// <summary>
        /// Pobieranie następnego meczu
        /// </summary>
        /// <returns>Mecz</returns>
        public Match GetNextMatch()
         {
             return dbMatch.MatchData.OrderByDescending(m => m.isCompleted).FirstOrDefault();
         }
    }
}