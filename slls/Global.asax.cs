using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MohammadYounes.Owin.Security.MixedAuth;
using slls.App_Settings;
using slls.DAO;
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
            // Event is raised each time a new session is created.
            
            // Get user's IP Address ...
            string ipAddress = HttpContext.Current.Request.UserHostAddress;
            
            //Insert into IpAddresses table if not exists ...
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
            db.Dispose(); 
            System.Web.HttpContext.Current.Session["ipAddressAllowed"] = IsIpAddressAllowed(ipAddress);
        }

        /// <summary> 
        /// Compares an IP address to list of valid IP addresses attempting to 
        /// find a match 
        /// </summary> 
        /// <param name="ipAddress">String representation of a valid IP Address</param> 
        /// <returns>Returns Boolean</returns> 
        private static bool IsIpAddressAllowed(string ipAddress)
        {
            //Split the user's IP address into its 4 octets (Assumes IPv4) 
            var incomingOctets = ipAddress.Trim().Split(new char[] { '.' });

            //Get the valid IP addresses from the database ...
            var allowedIpAddresses = CacheProvider.AllowedIpAddresses();

            //Get any blocked IP addresses from the database ...
            var blockedIpAddresses = CacheProvider.BlockedIpAddresses();

            if (blockedIpAddresses.Any())
            {
                // Iterate through each blocked IP address 
                foreach (var blockedIpAddress in allowedIpAddresses)
                {
                    // Return false if blocked IP address matches the user's  ...
                    if (blockedIpAddress.IpAddress1.Trim() == ipAddress)
                    {
                        return false;
                    }

                    // Split the blocked IP address into its 4 octets  ...
                    string[] validOctets = blockedIpAddress.IpAddress1.Trim().Split(new char[] { '.' });

                    var matches = true;

                    // Iterate through each octet ... 
                    for (var index = 0; index < validOctets.Length; index++)
                    {
                        //Skip if octet is an asterisk indicating an entire subnet range is blocked 
                        if (validOctets[index] != "*")
                        {
                            if (validOctets[index] == incomingOctets[index]) continue;
                            matches = false;
                            break; //Break out of loop 
                        }
                    }

                    if (matches)
                    {
                        return false;
                    }
                }
            }

            //IP not blocked, so now check whether IP Filtering is turned ON and, if so, whether or not the IP address is allowed ...
            if (!GlobalVariables.IpFilteringOn) return true;

            //Iterate through each allowed IP address ... 
            foreach (var allowedIpAddress in allowedIpAddresses)
            {
                //Return true if valid IP address matches the user's ... 
                if (allowedIpAddress.IpAddress1.Trim() == ipAddress) return true;

                //Split the valid IP address into its 4 octets ... 
                string[] validOctets = allowedIpAddress.IpAddress1.Trim().Split(new char[] { '.' });

                var matches = true;

                //Iterate through each octet  ...
                for (var index = 0; index < validOctets.Length; index++)
                {
                    //Skip if octet is an asterisk indicating an entire subnet range is valid ...
                    if (validOctets[index] == "*") continue;
                    if (validOctets[index] == incomingOctets[index]) continue;
                    matches = false;
                    break; //Break out of loop 
                }

                if (matches)
                {
                    return true;
                }
            }
            //Found no matches 
            return false;
            //IP filtering not implemented
        } 

    }
}