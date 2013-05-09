using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Membership.OpenAuth;

namespace ScheduleGeneratorProject
{
    internal static class AuthConfig
    {
        public static void RegisterOpenAuth()
        {
            // See http://go.microsoft.com/fwlink/?LinkId=252803 for details on setting up this ASP.NET
            // application to support logging in via external services.

            //OpenAuth.AuthenticationClients.AddTwitter(
            //    consumerKey: "your Twitter consumer key",
            //    consumerSecret: "your Twitter consumer secret");

            //New facebook app settings.
            OpenAuth.AuthenticationClients.AddFacebook(
                appId: "567506176604354",
                appSecret: "59466907af1d357917f4efe5e4798d5f");

            //OpenAuth.AuthenticationClients.AddFacebook(
            //    appId: "your Facebook app id",
            //    appSecret: "your Facebook app secret");

            //OpenAuth.AuthenticationClients.AddMicrosoft(
            //    clientId: "your Microsoft account client id",
            //    clientSecret: "your Microsoft account client secret");

           // OpenAuth.AuthenticationClients.AddGoogle();
        }
    }
}