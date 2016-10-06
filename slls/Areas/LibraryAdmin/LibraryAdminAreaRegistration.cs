using System.Web.Mvc;

namespace slls.Areas.LibraryAdmin
{
    public class LibraryAdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LibraryAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    name: "GetTitles",
            //    url: "Titles/GetTitles/",
            //    defaults: new { controller = "Titles", action = "GetTitles" },
            //    namespaces: new string[] { "slls.Areas.LibraryAdmin" }
            //);

            //context.MapRoute(
            //   name: "Admin_default",
            //   url: "Admin/{controller}/{action}/{id}",
            //   defaults: new { controller = "LibraryAdmin", action = "Index", id = UrlParameter.Optional },
            //   namespaces: new string[] { "slls.Areas.LibraryAdmin" }  // specify the new namespace
            //);

            context.MapRoute(
               name: "LibraryAdmin_default",
               url: "LibraryAdmin/{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
               namespaces: new string[] { "slls.Areas.LibraryAdmin" }
            );

           

        }
    }
}