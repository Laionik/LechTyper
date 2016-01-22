using LechTyper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LechTyper.Repository
{

    public class FixtureRepository
    { 
        private FixtureContext dbFixture;

        public FixtureRepository(FixtureContext dbFixture) 
        {
            this.dbFixture = dbFixture;
        }

        /// <summary>
        /// Najbliższa kolejka typerów
        /// </summary>
        /// <returns>Numer kolejki</returns>
        public int CurrentMatchDay()
        {
            return dbFixture.Fixtures.Where(f => f.homeGoal == null).Select(f => f.matchDay).FirstOrDefault();
        }

        /// <summary>
        /// Pobieranie terminarza danej kolejki
        /// </summary>
        /// <param name="matchDay">kolejka</param>
        /// <returns>Terminarz</returns>
        public List<Fixture> GetFixturesByMatchDay(int matchDay)
        {
            return dbFixture.Fixtures.Where(f => f.matchDay == matchDay).ToList();
        }

        /// <summary>
        /// Pobieranie listy użytkowników dla konkretnej kolejki
        /// </summary>
        /// <param name="fixtureList">Lista spotkań</param>
        /// <returns>Lista użytkowników</returns>
        public List<int> GetUserIdByFixture(List<Fixture> fixtureList)
        {
            var userIdList = fixtureList.Select(x => x.homeId).ToList();
            userIdList.AddRange(fixtureList.Select(x => x.guestId).ToList());
            return userIdList;
        }
    }
}