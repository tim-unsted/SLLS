using System;
using System.Data.Entity;
using System.Linq;
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

        protected void Session_Start(object sender, EventArgs e)
        {
            // event is raised each time a new session is created
            
            //Get users IP Address 
            string ipAddress = HttpContext.Current.Request.UserHostAddress;
            //Insert into IpAddresses if not exists
            var db = new DbEntities();
            var existing = db.IpAddresses.FirstOrDefault(x => x.IpAddress1 == ipAddress);
            if (existing == null)
            {
                var newIpAddress = new IpAddress()
                {
                    IpAddress1 = ipAddress,
                    AllowPassThrough = false,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
                db.IpAddresses.Add(newIpAddress);
                db.SaveChanges();
            }
        }

    }
}