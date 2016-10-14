using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using slls.Models;
using slls.Utils.Helpers;

namespace slls.Controllers
{
    public class sllsBaseController : Controller
    {
        //protected new JsonResult Json(object data)
        //{
        //    if (!Request.AcceptTypes.Contains("application/json"))
        //        return base.Json(data, "text/plain");
        //    else
        //        return base.Json(data);
        //}


        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User != null)
            {
                var context = new ApplicationDbContext();
                var currentUserId = Utils.PublicFunctions.GetUserId(); //User.Identity.GetUserId();
                
                //if (currentUserId == null)
                //{
                //    System.Security.Principal.IPrincipal xuser = System.Web.HttpContext.Current.User;
                //    var userName = Regex.Replace(xuser.Identity.Name,".*\\\\(.*)", "$1",RegexOptions.None); 
                //    user = context.Users.FirstOrDefault(u => u.UserName == userName);
                //}
                //else
                //{
                //    user = context.Users.Find(currentUserId);
                //}
                
                var user = context.Users.Find(currentUserId);

                if (user != null)
                {
                    string fullName = string.Concat(new[] { user.Firstname, " ", user.Lastname });
                    ViewData.Add("FirstName", user.Firstname);
                    ViewData.Add("FullName", fullName);
                }
            }
            base.OnActionExecuted(filterContext);
        }
        public sllsBaseController()
        { }
    }

}