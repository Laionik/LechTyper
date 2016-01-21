using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LechTyper.Models
{
    public class FixtureContext : DbContext
    {
        public FixtureContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Fixture> Fixtures { get; set; }
    }

    [Table("Fixture")]
    public class Fixture
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int id { get; set; }
        [Column("MatchDay")]
        public int matchDay { get; set; }
        [Column("HomeId")]
        public int homeId { get; set; }
        [Column("GuestId")]
        public int guestId { get; set; }
        [Column("HomeGoal")]
        public int? homeGoal { get; set; }
        [Column("GuestGoal")]
        public int? guestGoal { get; set; }


        public Fixture() { }

        public Fixture(int matchDay, int homeId, int guestId)
        {
            this.matchDay = matchDay;
            this.homeId = homeId;
            this.guestId = guestId;
            this.homeGoal = null;
            this.guestGoal = null;
        }

        public Fixture(int matchDay, int homeId, int guestId, int homeGoal, int guestGoal)
        {
            this.matchDay = matchDay;
            this.homeId = homeId;
            this.guestId = guestId;
            this.homeGoal = homeGoal;
            this.guestGoal = guestGoal;
        }
    }
}