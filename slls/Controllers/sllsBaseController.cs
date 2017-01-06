using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Win32;
using slls.App_Settings;
using slls.Models;
using slls.Utils;

namespace slls.Controllers
{
    public class sllsBaseController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var authMode =
                        ((AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication")).Mode;
            ViewData.Add("WindowsAuth", authMode == AuthenticationMode.Windows);

            if (User != null)
            {
                var context = new ApplicationDbContext();
                var currentUserId = PublicFunctions.GetUserId(); //User.Identity.GetUserId();
                
                //Find the user record in th database: AspNetUsers ...
                var user = context.Users.Find(currentUserId);

                if (user != null)
                {
                    var fullName = string.Concat(new[] { user.Firstname, " ", user.Lastname });
                    ViewData.Add("FirstName", user.Firstname);
                    ViewData.Add("FullName", fullName);
                }
                else //User record not yet in database, so add it now if using Windows Authentication: Get the details from AD ...
                {
                    //var authMode =
                    //    ((AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication")).Mode;
                    if (authMode == AuthenticationMode.Windows)
                    {
                        var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        var windowsUser = UserPrincipal.Current;
                        var defaultPasswordPart = Settings.GetParameterValue(
                            "Security.Passwords.DefaultPassweordPart", "6174",
                            "The numeric part of a default password when new users are added automatically, or via an import script or tool. A default password is constructed from a user's firstname, the default numeric part and the first letter of thier surname (e.g. tim1234u");
                
                        //Add a new Identity record and get the user's Id...
                        var identityUser = new ApplicationUser
                        {
                            UserName = Regex.Replace(windowsUser.SamAccountName, ".*\\\\(.*)", "$1", RegexOptions.None),
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
                            TempPassword = string.Format("{0}{1}{2}", windowsUser.GivenName, defaultPasswordPart, windowsUser.Surname.Substring(0, 1)).ToLower(), 
                        };
                        var result = userManager.Create(identityUser, identityUser.TempPassword);
                        if (result.Succeeded)
                        {
                            userManager.AddToRole(identityUser.Id, "OPAC User");
                        }
                        var fullName = string.Concat(new[] { windowsUser.GivenName, " ", windowsUser.Surname });
                        ViewData.Add("FirstName", windowsUser.GivenName);
                        ViewData.Add("FullName", fullName);
                    }
                }
            }
            base.OnActionExecuted(filterContext);
        }
        public sllsBaseController()
        { }

        public FileResult SendFileToBrowser(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string contentType = GetMimeType(fileName);
            return File(filePath, contentType, fileName);
        }

        public string GetMimeType(string fileName)
        {
            var mimeType = "text/plain";

            if (!string.IsNullOrEmpty(fileName))
            {
                String ext = Path.GetExtension(fileName).ToLower();
                RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(ext);

                if (regKey != null && regKey.GetValue("Content Type") != null)
                {
                    mimeType = regKey.GetValue("Content Type").ToString();
                }
                return mimeType;
            }
            return mimeType;
        }

    }

}