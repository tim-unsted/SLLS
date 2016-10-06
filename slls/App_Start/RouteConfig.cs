using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace slls
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            //Note: The following line must be BEFORE the RegisterAllAreas() function
            routes.MapMvcAttributeRoutes();

            AreaRegistration.RegisterAllAreas();

            //routes.MapRoute(
            //    name: "GetTitles2",
            //    url: "Titles/GetTitles/",
            //    defaults: new { controller = "Titles", action = "GetTitles" },
            //    namespaces: new string[] { "slls.Areas.LibraryAdmin" }
            //);
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] {"slls.Controllers"}
            );


        }
    }
}
