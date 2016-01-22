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
            DateTime defaultDate = DateTime.Parse("2000.01.01");
            return dbMatch.MatchData.Where(m => !m.isCompleted && m.date != defaultDate).OrderBy(m => m.date).ToList().First();
         }


        /// <summary>
        /// Pobieranie ostatniego meczu
        /// </summary>
        /// <returns>Mecz</returns>
        public Match GetLastMatch()
        {
            return dbMatch.MatchData.Where(m => m.isCompleted).OrderBy(m => m.date).ToList().Last();
        }
    }
}