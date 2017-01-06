using System.Web.Mvc;

namespace slls.Areas.CheckInOut
{
    public class CheckInOutAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CheckInOut";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: "CheckInOut_default",
                url: "CheckInOut/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "slls.Areas.CheckInOut" }
            );
        }
    }
}