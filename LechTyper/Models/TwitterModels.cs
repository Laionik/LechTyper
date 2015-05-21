﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LechTyper.Models
{
    public class ReadTwitter
    {
        public string created_at {get; set;}
        public string post_id {get; set;}
        public string text { get; set; }
        public string user_id {get; set;}
        public string user_name {get; set;}
        public string user_nick {get;set;}

        public ReadTwitter(string created_at, string post_id, string text, string user_id, string user_name, string user_nick)
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