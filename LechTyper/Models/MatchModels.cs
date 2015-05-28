using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LechTyper.Models
{
    public class MatchContext :DbContext
    {
        public MatchContext()
            : base("DefaultConnection")
        {
         }
        public DbSet<MatchContext> MatchDatas { get; set; }
    }


    [Table("Match")]
    public class Game
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MatchID { get; set; }
        public string date { get; set; }
        public string Competition { get; set; }
        public string Host { get; set; }
        public string Guest { get; set; }
        public int HTHostGoal { get; set; }
        public int HTGuestGoal { get; set; }
        public int FTHostGoal { get; set; }
        public int FTGuestGoal { get; set; }

        public Game(string dt, string comp, string host, string guest, int fthg, int ftgg)
        {
            this.date = dt;
            this.Competition = comp;
            this.Host = host;
            this.Guest = guest;
            this.FTHostGoal = fthg;
            this.FTGuestGoal = ftgg;
        }

        public Game(string dt, string comp, string host, string guest)
        {
            this.date = dt;
            this.Competition = comp;
            this.Host = host;
            this.Guest = guest;
        }
    }
}