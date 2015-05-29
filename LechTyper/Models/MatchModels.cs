using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LechTyper.Models
{
    public class GameContext : DbContext
    {
        public GameContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Game> GameData { get; set; }
    }

    [Table("Game")]
    public class Game
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MatchID { get; set; }
        [Display(Name = "Data rozegrania meczu")]
        public string date { get; set; }
        [Display(Name = "Rozgrywki")]
        public string Competition { get; set; }
        [Display(Name = "Gospodarz")]
        public string Host { get; set; }
        [Display(Name = "Gość")]
        public string Guest { get; set; }
        [Display(Name = "Barmki gospodarzy")]
        public int FTHostGoal { get; set; }
        [Display(Name = "Bramki gości")]
        public int FTGuestGoal { get; set; }
        [Display(Name = "Stan")]
        public bool isCompleted { get; set; }

        public Game() { }
        public Game(string dt, string comp, string host, string guest, int hostgoal, int guestgoal, bool isCompleted)
        {
            this.date = dt;
            this.Competition = comp;
            this.Host = host;
            this.Guest = guest;
            this.FTHostGoal = hostgoal;
            this.FTGuestGoal = guestgoal;
            this.isCompleted = isCompleted;
        }

        public Game(string dt, string comp, string host, string guest, bool isCompleted)
        {
            this.date = dt;
            this.Competition = comp;
            this.Host = host;
            this.Guest = guest;
            this.isCompleted = isCompleted;
        }
    }
}