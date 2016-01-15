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
        public DbSet<Tweet> Tweets { get; set; }
    }

    [Table("Twitter")]
    public class Tweet
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }
        [Column("TweetId")]
        public string tweetId { get; set; }
        [Column("UserName")]
        public string userName { get; set; }
        [Column("Result")]
        public string result { get; set; }
        [Column("ResultHalf")]
        public string resultHalf { get; set; }
        [Column("Scorer")]
        public string scorer { get; set; }
        [Column("PostDate")]
        public DateTime postDate { get; set; }

        public Tweet() { }
        public Tweet(string tweetId, string userName, string result, string resultHalf, string scorer, DateTime postDate)
        {
            this.tweetId = tweetId;
            this.userName = userName;
            this.result = result;
            this.resultHalf = resultHalf;
            this.scorer = scorer;
            this.postDate = DateTime.Parse(postDate.ToShortDateString());
        }
    }
}