using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Win32;
using slls.App_Settings;
using slls.Filters;
using slls.Models;
using slls.Utils;

namespace slls.Controllers
{
    [AuthorizeIpAddress]
    public class sllsBaseController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var authMode =
                        ((AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication")).Mode;
            ViewData.Add("WindowsAuth", authMode == AuthenticationMode.Windows);

            if (User != null)
            {
                if (Session["currentUserId"] != null)
                {
                    ViewData.Add("FirstName", Session["userFirstName"].ToString());
                    ViewData.Add("FullName", Session["userFirstName"].ToString());
                }
                else
                {
                    var currentUserId = PublicFunctions.GetUserId(); //User.Identity.GetUserId();
                    if (currentUserId != null)
                    {
                        //Find the user record in th database: AspNetUsers ...
                        var context = new ApplicationDbContext();
                        var user = context.Users.Find(currentUserId);

                        if (user != null)
                        {
                            var fullName = string.Concat(new[] { user.Firstname, " ", user.Lastname });
                            Session["currentUserId"] = currentUserId;
                            Session["userFirstName"] = user.Firstname;
                            Session["userFullName"] = fullName;
                            ViewData.Add("FirstName", user.Firstname);
                            ViewData.Add("FullName", fullName);
                        }
                        else //User record not yet in database, so add it now if using Windows Authentication: Get the details from AD ...
                        {
                            if (authMode == AuthenticationMode.Windows)
                            {
                                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                                //var windowsUser = UserPrincipal.Current;
                                var ctx = new PrincipalContext(ContextType.Domain);
                                var windowsUser = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);
                                var defaultPasswordPart = Settings.GetParameterValue(
                                    "Security.Passwords.DefaultPasswordPart", "6174",
                                    "The numeric part of a default password when new users are added automatically, or via an import script or tool. A default password is constructed from a user's firstname, the default numeric part and the first letter of thier surname (e.g. tim1234u)",
                                    dataType: "int");

                                //Add a new Identity record and get the user's Id...
                                if (windowsUser != null)
                                {
                                    var identityUser = new ApplicationUser
                                    {
                                        UserName =
                                            Regex.Replace(windowsUser.SamAccountName, ".*\\\\(.*)", "$1", RegexOptions.None),
                                        LibraryUserId = 0,
                                        Email = windowsUser.EmailAddress,
                                        AdObjectGuid = windowsUser.Guid.ToString(),
                                        Firstname = windowsUser.GivenName,
                                        Lastname = windowsUser.Surname,
                                        UserBarcode = "",
                                        SelfLoansAllowed = true,
                                        IgnoreAd = false,
                                        IsLive = true,
                                        FoundInAd = true,
                                        InputDate = DateTime.Now,
                                        CanDelete = true,
                                        TempPassword =
                                            string.Format("{0}{1}{2}", windowsUser.GivenName, defaultPasswordPart,
                                                windowsUser.Surname.Substring(0, 1)).ToLower(),
                                    };
                                    var result = userManager.Create(identityUser, identityUser.TempPassword);
                                    if (result.Succeeded)
                                    {
                                        userManager.AddToRole(identityUser.Id, "OPAC User");
                                        var provider = new WindowsTokenRoleProvider();
                                        var roles = provider.GetRolesForUser(User.Identity.Name);
                                        //if (roles.Any(role => provider.IsUserInRole(User.Identity.Name, "Administrator")))
                                        //{
                                        //    return;
                                        //}
                                        if (roles.Any(role => role.Contains("Administrator")))
                                        {
                                            userManager.AddToRole(identityUser.Id, "System Admin");
                                        }
                                    }
                                    var fullName = string.Concat(new[] { windowsUser.GivenName, " ", windowsUser.Surname });
                                    Session["currentUserId"] = identityUser.Id;
                                    Session["userFirstName"] = windowsUser.GivenName;
                                    Session["userFullName"] = fullName;
                                    ViewData.Add("FirstName", windowsUser.GivenName);
                                    ViewData.Add("FullName", fullName);
                                }
                            }
                        }
                    }
                }
            }
            base.OnActionExecuted(filterContext);
        }
    }

}