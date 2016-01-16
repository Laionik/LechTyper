using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LechTyper.Models
{
    public class LeagueContext : DbContext
    {
        public LeagueContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<League> Leagues { get; set; }
    }

    [Table("League")]
    public class League
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int id { get; set; }
        [Column("UserId")]
        public int userId { get; set; }
        [Column("Points")]
        public int points { get; set; }
        [Column("Matches")]
        public int matches { get; set; }
        [Column("GoalScored")]
        public int goalScored { get; set; }
        [Column("GoalConceded")]
        public int goalConceded { get; set; }
        [Column("Win")]
        public int win { get; set; }
        [Column("Draw")]
        public int draw { get; set; }
        [Column("Lose")]
        public int lose { get; set; }
        [Column("Division")]
        public int division { get; set; }

        public League() { }
        public League(int userId, int division)
        {
            this.userId = userId;
            this.points = 0;
            this.matches = 0;
            this.goalScored = 0;
            this.goalConceded = 0;
            this.win = 0;
            this.draw = 0;
            this.lose = 0;
            this.division = division;
        }
        public League(int userId, int points, int matches, int goalScored, int goalConceded, int win, int draw, int lose, int division)
        {
            this.userId = userId;
            this.points = points;
            this.matches = matches;
            this.goalScored = goalScored;
            this.goalConceded = goalConceded;
            this.win = win;
            this.draw = draw;
            this.lose = lose;
            this.division = division;
        }
    }
}