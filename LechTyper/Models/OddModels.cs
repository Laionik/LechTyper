using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LechTyper.Models
{
    public class OddContext : DbContext
    {
        public OddContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Odd> Odds { get; set; }
    }


    [Table("Odd")]
    public class Odd
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OddID { get; set; }
        public int MatchID { get; set; }
        [Display(Name="Wygrana gospodarzy")]
        public double odd_1 { get; set; }
        [Display(Name = "Remis")]
        public double odd_x { get; set; }
        [Display(Name = "Wygrana gości")]
        public double odd_2 { get; set; }

        public Odd() { }

        public Odd(int matchid, double odd_1, double odd_x, double odd_2)
        {
            this.MatchID = matchid;
            this.odd_1 = odd_1;
            this.odd_2 = odd_2;
            this.odd_x = odd_x;
        }
    }

}