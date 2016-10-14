using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;

namespace slls.Utils.Helpers
{
    public static class Roles
    {
        public const string Administrator = "Admin";
        public const string Staff = "Library Staff";
        public const string User = "OPAC User";
        public const string Anon = "Anonymous";
        public const string BsAdmin = "Bailey Admin";
        //public const string BsDeveloper = "Bailey Developer";

        public static IList<string> GetUserRoles()
        {
            var userId = Utils.PublicFunctions.GetUserId(); //HttpContext.Current.User.Identity.GetUserId();
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            if (userId == null)
            {
                return null; 
            }
            var userRoles = userManager.GetRoles(userId);
            return userRoles;
        }

        public static IList<string> GetUserRoles(string id)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var userRoles = userManager.GetRoles(id);
            return userRoles;
        }

        public static bool IsLibraryStaff()
        {
            var userRoles = GetUserRoles();
            return userRoles != null && userRoles.Contains(Staff);
        }

        public static bool IsAdmin()
        {
            var userRoles = GetUserRoles();
            return userRoles.Contains(Administrator);
        }

        public static bool IsBaileyAdmin()
        {
            var userRoles = GetUserRoles();
            return userRoles.Contains(BsAdmin);
        }
    }

    
}