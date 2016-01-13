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
                consumerKey: "EpfmvWrsmtncANw5Rfsn4xMQ3",
                consumerSecret: "0gdIryMSHwVYMvyx8Jj1K0ncx71Apjep0JHOpSuBVfRw1owSMQ");

            //Consumer Key (API Key)	YU2hNYSeRoTP1MTLssEYvWOQ7
            //Consumer Secret (API Secret)	LAqWMb2FZiG8lNuLbSKBlfWiTYNcBGiYSNh5OPildizuZpkaC6
            //Access Token	860293476-g6vu3t9709eQfiHTjZ2sBTAjicpvwTkcV3d2wzzP
            //Access Token Secret	9zhTFHuZuclkVzVJ8gZdrqenWWLLIiQKx0pcjCQX69nWP

            Auth.SetUserCredentials("YU2hNYSeRoTP1MTLssEYvWOQ7", "LAqWMb2FZiG8lNuLbSKBlfWiTYNcBGiYSNh5OPildizuZpkaC6", "860293476-g6vu3t9709eQfiHTjZ2sBTAjicpvwTkcV3d2wzzP", "9zhTFHuZuclkVzVJ8gZdrqenWWLLIiQKx0pcjCQX69nWP");

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: "346506492212997",
            //    appSecret: "c1c2b010a291a8ba3f5c5c4983aef339");

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
