using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using LechTyper.Models;

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

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: "346506492212997",
            //    appSecret: "c1c2b010a291a8ba3f5c5c4983aef339");

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
