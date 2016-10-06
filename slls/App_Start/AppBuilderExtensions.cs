using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using MohammadYounes.Owin.Security.MixedAuth;
using Owin;
using slls.Models;

namespace slls
{
    public static class AppBuilderExtensions
    {
        public static void ApplyOwinConfigSettings(this IAppBuilder app)
        {
            var config = (OWinConfigSection)System.Configuration.ConfigurationManager.GetSection("ExternalLogins");
            if (config == null) return;

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            var cookieOptions = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                ExpireTimeSpan = TimeSpan.FromDays(90),
                SlidingExpiration = true,
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            };
            app.UseCookieAuthentication(cookieOptions);
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Check which external authentication methods are available to us ...
            if (config.MicrosoftAccountAuthentication.Enabled) app.UseMicrosoftAccountAuthentication(
                clientId: config.MicrosoftAccountAuthentication.ClientId,
                clientSecret: config.MicrosoftAccountAuthentication.ClientSecret
            );

            if (config.TwitterAuthentication.Enabled) app.UseTwitterAuthentication(
                consumerKey: config.TwitterAuthentication.ConsumerKey,
                consumerSecret: config.TwitterAuthentication.ConsumerSecret
            );

            if (config.FacebookAuthentication.Enabled) app.UseFacebookAuthentication(
                appId: config.FacebookAuthentication.AppId,
                appSecret: config.FacebookAuthentication.AppSecret
            );

            if (config.GoogleAuthentication.Enabled) app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = config.GoogleAuthentication.ClientId,
                ClientSecret = config.GoogleAuthentication.ClientSecret
            });

            if (config.MixedModeAuthentication.Enabled) app.UseMixedAuth(new MixedAuthOptions()
            {
                Provider = new MixedAuthProvider()
                {
                    OnImportClaims = identity =>
                    {
                        var claims = new List<Claim>();

                        try
                        {
                            using (var principalContext = new PrincipalContext(ContextType.Domain)) //or ContextType.Machine
                            {
                                using (var userPrincipal = UserPrincipal.FindByIdentity(principalContext, identity.Name))
                                {
                                    if (userPrincipal != null)
                                    {
                                        claims.Add(new Claim(ClaimTypes.Email, userPrincipal.EmailAddress ?? string.Empty));
                                        claims.Add(new Claim(ClaimTypes.Surname, userPrincipal.Surname ?? string.Empty));
                                        claims.Add(new Claim(ClaimTypes.GivenName, userPrincipal.GivenName ?? string.Empty));
                                    }
                                }
                            }
                            return claims;
                        }
                        catch (Exception)
                        {
                            //return null;
                            return claims;
                            //throw;
                        }
                    }
                }

            }, cookieOptions);
        }
    }

    public class OWinConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Microsoft", IsRequired = false)]
        public MicrosoftAccountConfigurationElement MicrosoftAccountAuthentication
        { get { return (MicrosoftAccountConfigurationElement)this["Microsoft"]; } }

        [ConfigurationProperty("Twitter", IsRequired = false)]
        public TwitterConfigurationElement TwitterAuthentication
        { get { return (TwitterConfigurationElement)this["Twitter"]; } }

        [ConfigurationProperty("Facebook", IsRequired = false)]
        public FacebookConfigurationElement FacebookAuthentication
        { get { return (FacebookConfigurationElement)this["Facebook"]; } }

        [ConfigurationProperty("Google", IsRequired = false)]
        public GoogleConfigurationElement GoogleAuthentication
        { get { return (GoogleConfigurationElement)this["Google"]; } }

        [ConfigurationProperty("MixedMode", IsRequired = false)]
        public MixedModeConfigurationElement MixedModeAuthentication
        { get { return (MixedModeConfigurationElement)this["MixedMode"]; } }
    }

    public class MicrosoftAccountConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("Enabled", DefaultValue = "false", IsRequired = false)]
        public bool Enabled
        { get { return (bool)this["Enabled"]; } set { this["enabled"] = value; } }

        [ConfigurationProperty("clientId", DefaultValue = "", IsRequired = true)]
        public string ClientId
        { get { return (string)this["clientId"]; } set { this["clientId"] = value; } }

        [ConfigurationProperty("clientSecret", DefaultValue = "", IsRequired = true)]
        public string ClientSecret
        { get { return (string)this["clientSecret"]; } set { this["ClientSecret"] = value; } }
    }

    public class TwitterConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("Enabled", DefaultValue = "false", IsRequired = false)]
        public bool Enabled
        { get { return (bool)this["Enabled"]; } set { this["enabled"] = value; } }

        [ConfigurationProperty("consumerKey", DefaultValue = "", IsRequired = true)]
        public string ConsumerKey
        { get { return (string)this["consumerKey"]; } set { this["consumerKey"] = value; } }

        [ConfigurationProperty("consumerSecret", DefaultValue = "", IsRequired = true)]
        public string ConsumerSecret
        { get { return (string)this["consumerSecret"]; } set { this["consumerSecret"] = value; } }
    }

    public class FacebookConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("Enabled", DefaultValue = "false", IsRequired = false)]
        public bool Enabled
        { get { return (bool)this["Enabled"]; } set { this["Enabled"] = value; } }

        [ConfigurationProperty("appId", DefaultValue = "", IsRequired = true)]
        public string AppId
        { get { return (string)this["appId"]; } set { this["appId"] = value; } }

        [ConfigurationProperty("appSecret", DefaultValue = "", IsRequired = true)]
        public string AppSecret
        { get { return (string)this["appSecret"]; } set { this["appSecret"] = value; } }
    }

    public class GoogleConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("Enabled", DefaultValue = "false", IsRequired = false)]
        public bool Enabled
        { get { return (bool)this["Enabled"]; } set { this["Enabled"] = value; } }

        [ConfigurationProperty("ClientId", DefaultValue = "", IsRequired = true)]
        public string ClientId
        { get { return (string)this["ClientId"]; } set { this["ClientId"] = value; } }

        [ConfigurationProperty("ClientSecret", DefaultValue = "", IsRequired = true)]
        public string ClientSecret
        { get { return (string)this["ClientSecret"]; } set { this["ClientSecret"] = value; } }
    }

    public class MixedModeConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("Enabled", DefaultValue = "false", IsRequired = false)]
        public bool Enabled
        { get { return (bool)this["Enabled"]; } set { this["enabled"] = value; } }
    }
}