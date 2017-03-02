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
        
        public const string Anon = "Anonymous";
        public const string User = "OPAC User";
        public const string CatalogueAdmin = "Catalogue Admin";
        public const string LoansAdmin = "Loans Admin";
        public const string FinanceAdmin = "Finance Admin";
        public const string SerialsAdmin = "Serials Admin";
        public const string UsersAdmin = "Users Admin";
        public const string OpacAdmin = "OPAC Admin";
        public const string SystemAdmin = "System Admin";
        public const string BaileyAdmin = "Bailey Admin";

        public static IList<string> GetUserRoles()
        {
            if (HttpContext.Current.Session["UserRoles"] != null)
            {
                return (IList<string>)HttpContext.Current.Session["UserRoles"];
            }
            var userId = Utils.PublicFunctions.GetUserId();
            if (userId == null) return null;
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var userRoles = userManager.GetRoles(userId);
            HttpContext.Current.Session["UserRoles"] = userRoles;
            return userRoles;
        }

        public static IList<string> GetUserRoles(string id)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var userRoles = userManager.GetRoles(id);
            return userRoles;
        }

        public static bool IsLibraryStaff() // Various roles included ...
        {
            var userRoles = GetUserRoles();
            return userRoles != null && (userRoles.Contains(CatalogueAdmin) || userRoles.Contains(FinanceAdmin) || userRoles.Contains(LoansAdmin) || userRoles.Contains(SerialsAdmin) || userRoles.Contains(UsersAdmin));
        }

        public static bool IsAdmin() // System Admin ...
        {
            var userRoles = GetUserRoles();
            if (userRoles == null)
            {
                return false;
            }
            return userRoles.Contains(SystemAdmin);
        }

        public static bool IsBaileyAdmin() // Bailey Admin ...
        {
            var userRoles = GetUserRoles();
            if (userRoles == null)
            {
                return false;
            }
            return userRoles.Contains(BaileyAdmin);
        }

        public static bool IsUserInRole(string roleName)
        {
            if (IsBaileyAdmin())
            {
                return true;
            }
            var userRoles = GetUserRoles();
            if (userRoles == null)
            {
                return false;
            }
            return userRoles.Contains(roleName);
        }
    }

    
}