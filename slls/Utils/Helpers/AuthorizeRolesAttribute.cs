using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace slls.Utils.Helpers
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles)
            : base()
        {
            Roles = string.Join(",", roles);
        }
        
        //public bool IsValidUser { get; protected set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            base.AuthorizeCore(httpContext);
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            IPrincipal user = httpContext.User;

            // Make sure Forms authentication shows the user as authenticated - Only works for FORMS auth, not for WINDOWS auth
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            //string user = httpContext.Request.ServerVariables["LOGON_USER"];

            //if (user != null)
            //    IsValidUser = true;

            //return IsValidUser;

            if (Roles.Length == 0)
            {
                return false;
            }

            var userRoles = Helpers.Roles.GetUserRoles();
            return Roles.Split(new String[] {","}, StringSplitOptions.RemoveEmptyEntries)
                .Any(x => userRoles.Contains(x));
        }

    }
}