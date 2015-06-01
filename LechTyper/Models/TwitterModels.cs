using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LechTyper.Models
{
    public class TwitterContext : DbContext
    {
        public TwitterContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Twitt> Twitts { get; set; }
    }

    [Table("Twitt")]
    public class Twitt
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("Twittid")]
        public int Twittid { get; set; }
        [Column("Created_at")]
        public string created_at { get; set; }
        [Column("Post_id")]
        public string post_id { get; set; }
        [Column("Text")]
        public string text { get; set; }
        [Column("User_id")]
        public string user_id { get; set; }
        [Column("User_name")]
        public string user_name { get; set; }
        [Column("User_nick")]
        public string user_nick { get; set; }

        public Twitt() { }
        public Twitt(string created_at, string post_id, string text, string user_id, string user_name, string user_nick)
        {
            this.created_at = created_at;
            this.post_id = post_id;
            this.text = text;
            this.user_id = user_id;
            this.user_name = user_name;
            this.user_nick = user_nick;
        }
    }
}