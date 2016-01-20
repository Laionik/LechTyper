using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LechTyper.Models
{
    public class MatchContext : DbContext
    {
        public MatchContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Match> MatchData { get; set; }
    }

    [Table("Match")]
    public class Match
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int id { get; set; }
        [Column("Date")]
        public DateTime date { get; set; }
        [Column("Competition")]
        public string competition { get; set; }
        [Column("Host")]
        public string host { get; set; }
        [Column("Guest")]
        public string guest { get; set; }
        [Column("FinalHostGoal")]
        public int finalHostGoal { get; set; }
        [Column("FinalGuestGoal")]
        public int finalGuestGoal { get; set; }
        [Column("HalfHostGoal")]
        public int halfHostGoal { get; set; }
        [Column("HalfGuestGoal")]
        public int halfGuestGoal { get; set; }
        [Column("Scorers")]
        public string scorers { get; set; }
        [Column("IsCompleted")]
        public bool isCompleted { get; set; }

        public Match() { }
        public Match(DateTime date, string competition, string host, string guest, int finalHostGoal, int finalGuestGoal, int halfHostGoal, int halfGuestGoal, string scorers, bool isCompleted)
        {
            this.date = date;
            this.competition = competition;
            this.host = host;
            this.guest = guest;
            this.finalHostGoal = finalHostGoal;
            this.finalGuestGoal = finalGuestGoal;
            this.halfHostGoal = halfHostGoal;
            this.halfGuestGoal = halfGuestGoal;
            this.scorers = scorers;
            this.isCompleted = isCompleted;
        }

        public Match(DateTime date, string competition, string host, string guest, int finalHostGoal, int finalGuestGoal, bool isCompleted)
        {
            this.date = date;
            this.competition = competition;
            this.host = host;
            this.guest = guest;
            this.finalHostGoal = finalHostGoal;
            this.finalGuestGoal = finalGuestGoal;
            this.isCompleted = isCompleted;
        }

        public Match(DateTime date, string competition, string host, string guest, bool isCompleted)
        {
            this.date = date;
            this.competition = competition;
            this.host = host;
            this.guest = guest;
            this.isCompleted = isCompleted;
        }
    }
}