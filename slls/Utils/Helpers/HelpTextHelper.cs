using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using slls.DAO;
using slls.Models;

namespace slls.Utils.Helpers
{
    public class HelpTextHelper
    {
        public static string GetHelpText(string helpId = "")
        {
            if (string.IsNullOrEmpty(helpId))
            {
                return string.Empty;
            }
            //using (var db = new DbEntities())
            //{
            //    var helptext = db.HelpTexts.FirstOrDefault(x => x.HelpId == helpId);
            //    if (helptext == null)
            //    {
            //        return string.Empty;
            //    }
            //    return helptext.HelpTextString;
            //}
            var allHelpText = CacheProvider.GetAll<HelpText>("helptexts");
            var helptext = allHelpText.FirstOrDefault(x => x.HelpId == helpId);
            return helptext == null ? string.Empty : helptext.HelpTextString;
        }
    }
}