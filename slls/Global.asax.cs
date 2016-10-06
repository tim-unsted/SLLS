using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MohammadYounes.Owin.Security.MixedAuth;
using slls.Models;

namespace slls
{
    public class MvcApplication : HttpApplication
    {
        public MvcApplication()
        {
            this.RegisterMixedAuth();
        }

        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer<ApplicationDbContext>(null);
        }
    }
}