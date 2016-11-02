using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using slls.Models;
using Microsoft.AspNet.Identity;

namespace slls.Utils
{
    public class PublicFunctions
    {
        public static string NewBarcode()
        {
            //Generate a new unique barcode ...
            DateTime timeBegin = new DateTime(2016, 7, 29);
            DateTime currentDate = DateTime.Now;
            long newBarcode = (currentDate.Ticks - timeBegin.Ticks) / 1000000;
            return newBarcode.ToString();
        }

        public static int GetDefaultValue(string entity = "", string field = "")
        {
            if (string.IsNullOrEmpty(entity)) return 1;
            if (string.IsNullOrEmpty(field)) return 1;

            var db = new DbEntities();

            var defaultValue = db.DefaultValues.FirstOrDefault(
                d =>
                    d.TableName.Equals(entity, StringComparison.OrdinalIgnoreCase) &&
                    d.FieldName.Equals(field, StringComparison.OrdinalIgnoreCase));
                               
            return defaultValue != null ? defaultValue.DefaultValueId : 1;
        }

        public static int GetDefaultLoanType(int mediaId = 0)
        {
            if (mediaId == 0) return 1;
            var db = new DbEntities();
            var mediaType = db.MediaTypes.Find(mediaId);
            return mediaType != null ? mediaType.LoanTypeID : 1;
        }

        public static string GetCurrentUserName()
        {
            string userName = "";
            //var currentUserId = HttpContext.Current.User.Identity.GetUserId();
            var currentUserId = Utils.PublicFunctions.GetUserId();
            if (currentUserId == null) return userName;
            var db = new DbEntities();
            var user = db.Users.Find(currentUserId);
            if (user != null)
            {
                userName = string.Concat(new[] { user.Lastname, ", ", user.Firstname });
            }
            return userName;
        }

        public static string GetUserId()
        {
            var context = new ApplicationDbContext();
            System.Security.Principal.IPrincipal user = System.Web.HttpContext.Current.User;
            var userName = Regex.Replace(user.Identity.Name, ".*\\\\(.*)", "$1", RegexOptions.None);
            var currentUser = context.Users.FirstOrDefault(u => u.UserName == userName);
            if (currentUser != null)
            {
                return currentUser.Id;
            }
            return null;
        }
    }
}