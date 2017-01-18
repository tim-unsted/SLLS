using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using slls.App_Settings;
using slls.DAO;
using slls.Models;

namespace slls.Filters
{
    public class AuthorizeIpAddressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Check that the current user's IP Address is allowed. 
            // This is stored in a session variable that gets set in Session_Start() in Global.asax...
            var allowed = (bool) (HttpContext.Current.Session["ipAddressAllowed"]);
            
            if (!allowed)
                {
                    // Send back a HTTP Status code of 403 Forbidden ... 
                    filterContext.Result = new HttpStatusCodeResult(403);
                }

            //Otherwise, allow the user to proceed ...
            base.OnActionExecuting(filterContext);
        }

        
    }
}