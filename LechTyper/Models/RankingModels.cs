using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LechTyper.Models
{
    public class RankingContext : DbContext
    {
        public RankingContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Rank> Ranks { get; set; }
    }

    [Table("Ranking")]
    public class Rank
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RankID { get; set; }
        public string UserID { get; set; }
        [Display(Name = "Stan konta")]
        public double Balance { get; set; }
        [Display(Name = "Dokładne wyniki")]
        public int Bet_exact { get; set; }
        [Display(Name = "1/x/2")]
        public int Hda { get; set; }
        [Display(Name = "Najwyższa wygrana")]
        public double Prize { get; set; }
        [Display(Name = "Średnia stawka")]
        public double Bet_avg { get; set; }
        [Display(Name = "Liczba typów")]
        public int Played_number { get; set; }
        public Rank() { }
        public Rank(string uid, double balance, int bet_ex, int gda, double prize, double bet_avg, int pl_number)
        {
            this.UserID = uid;
            this.Balance = balance;
            this.Bet_exact = bet_ex;
            this.Bet_avg = bet_avg;
            this.Hda = Hda;
            this.Prize = prize;
            this.Played_number = pl_number;
        }
    }
}