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
                //var db = new DbEntities();
                var context = new ApplicationDbContext();
                //var username = User.Identity.Name;
                var currentUserId = User.Identity.GetUserId();

                if (currentUserId != null)
                {
                    //var user = context.Users.SingleOrDefault(u => u.UserName == username);
                    var user = context.Users.Find(currentUserId);
                    if (user != null)
                    {
                        string fullName = string.Concat(new[] { user.Firstname, " ", user.Lastname });
                        ViewData.Add("FirstName", user.Firstname);
                        ViewData.Add("FullName", fullName);
                    }
                }
            }
            base.OnActionExecuted(filterContext);
        }
        public sllsBaseController()
        { }
    }

}