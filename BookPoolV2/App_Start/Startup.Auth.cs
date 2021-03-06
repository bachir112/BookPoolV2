﻿using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using BookPoolV2.Models;
using System.Threading.Tasks;

namespace BookPoolV2
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //dev
            //var facebookOptions = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions()
            //{
            //    AppId = "571875340131395",
            //    AppSecret = "279cbbfcf505ab7eb0566936796007cb",
            //    Provider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider()
            //    {
            //        OnAuthenticated = (context) =>
            //        {
            //            context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:access_token", context.AccessToken, "Facebook"));
            //            context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:email", context.Email, "Facebook"));
            //            context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:email", context.Email, "Facebook"));
            //            return Task.FromResult(0);
            //        }
            //    }
            //};

            //prod
            var facebookOptions = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions()
            {
                AppId = "292878855237237",
                AppSecret = "34800344467e945a987c6e4d32527902",
                Provider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider()
                {
                    OnAuthenticated = (context) =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:access_token", context.AccessToken, "Facebook"));
                        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:email", context.Email, "Facebook"));
                        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:email", context.Email, "Facebook"));
                        return Task.FromResult(0);
                    }
                }
            };

            facebookOptions.Scope.Add("email");

            app.UseFacebookAuthentication(facebookOptions);

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //prod
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "337728385862-uso50m3m8slhma133686ec7stmi17bp1.apps.googleusercontent.com",
                ClientSecret = "YzQnPSohrfZypd-WbuVRo1jk"
            });

            //dev
            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "337728385862-q8031mo7q2qk15d94e2a9kv47st1ahkb.apps.googleusercontent.com",
            //    ClientSecret = "IG0Vh2iIRkfrd0dPSmeJtpNw"
            //});
        }
    }
}