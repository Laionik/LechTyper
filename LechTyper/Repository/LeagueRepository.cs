using LechTyper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LechTyper.Repository
{
    public class LeagueRepository
    {
        private LeagueContext dbLeague;
        public LeagueRepository(LeagueContext dbLeague) 
        {
            this.dbLeague = dbLeague;
        }

        /// <summary>
        /// Pobieranie listy poziomów rozgrywkowych
        /// </summary>
        /// <returns>Lista poziomów rozgrywkowych</returns>
        public List<int> GetDivisions()
        {
            return dbLeague.Leagues.Select(d => d.division).Distinct().ToList();
        }
        /// <summary>
        /// Pobieranie tabel poziomów rozgrywkowych
        /// </summary>
        /// <returns>Wpisy tabel ligowych</returns>
        public List<League> GetLeagues()
        {
            return dbLeague.Leagues.OrderBy(d => d.division).ToList();
        }
    }
}