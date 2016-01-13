using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using LechTyper.Models;
using Tweetinvi;

namespace LechTyper
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");
            
            OAuthWebSecurity.RegisterTwitterClient(
                consumerKey: "pKx5DJEliHoCCCIUt1j9JEyMX",
                consumerSecret: "NqQqYfQQ7e3ZhSRQxccaz66LUqXL5u5WvaYEJ4eg1a5U4i0vHC");


            //Laionik
            //Consumer Key (API Key)	YU2hNYSeRoTP1MTLssEYvWOQ7
            //Consumer Secret (API Secret)	LAqWMb2FZiG8lNuLbSKBlfWiTYNcBGiYSNh5OPildizuZpkaC6
            //Access Token	860293476-g6vu3t9709eQfiHTjZ2sBTAjicpvwTkcV3d2wzzP
            //Access Token Secret	9zhTFHuZuclkVzVJ8gZdrqenWWLLIiQKx0pcjCQX69nWP



            //LechTyperDevTest
            //Consumer Key (API Key)	"pKx5DJEliHoCCCIUt1j9JEyMX"
            //Consumer Secret (API Secret)	"NqQqYfQQ7e3ZhSRQxccaz66LUqXL5u5WvaYEJ4eg1a5U4i0vHC"
            //Access Token	"3208367135-4TmOMDlJA1RswLiYMZyckgL3tPZfMbBbQhf2Kot"
            //Access Token Secret	"vmlp7rpNhtcPtusTaRR9ko02vqFGLzmkOUrVdrwHdo8Rh"

            Auth.SetUserCredentials("pKx5DJEliHoCCCIUt1j9JEyMX", "NqQqYfQQ7e3ZhSRQxccaz66LUqXL5u5WvaYEJ4eg1a5U4i0vHC", "3208367135-4TmOMDlJA1RswLiYMZyckgL3tPZfMbBbQhf2Kot", "vmlp7rpNhtcPtusTaRR9ko02vqFGLzmkOUrVdrwHdo8Rh");

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: "346506492212997",
            //    appSecret: "c1c2b010a291a8ba3f5c5c4983aef339");

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
