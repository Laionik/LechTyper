using LechTyper.Models;
using System;
using System.Collections.Generic;
using System.Data.Objects;
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

        /// <summary>
        /// Liczba dni od poprzedniego meczu
        /// </summary>
        /// <returns>Mecz</returns>
        public int GetLastMatchDays()
        {
            var match = dbMatch.MatchData.Where(m => m.isCompleted).OrderBy(m => m.date).ToList().Last();
            return match.date.Subtract(DateTime.Now).Days;
        }


        /// <summary>
        /// Wyszukiwanie meczu po ID
        /// </summary>
        /// <returns>Mecz</returns>
        public Match GetMatchById(int id)
        {
            return dbMatch.MatchData.Where(m => m.id == id).FirstOrDefault();
        }
        /// <summary>
        /// Liczba dni do kolejnego meczu
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Liczba dni</returns>
        public int GetDaysToNextMatch()
        {
            var match = dbMatch.MatchData.Where(m => EntityFunctions.DiffDays(m.date, DateTime.Now) < 0).OrderBy(m => m.date).ToList().FirstOrDefault();
            return match.date.Subtract(DateTime.Now).Days;
        }
    }
}