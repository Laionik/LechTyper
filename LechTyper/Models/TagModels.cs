using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LechTyper.Models
{
    public class TagContext : DbContext
    {
        public TagContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Tag> Tags { get; set; }
    }

    [Table("Tag")]
    public class Tag
    {
         [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TagID { get; set; }
        public string TagText { get; set; }
        public int MatchID { get; set; }

        public Tag() { }
        public Tag(string tag, int matchid)
        {
            this.TagText = tag;
            this.MatchID = matchid;
        }
    }
}