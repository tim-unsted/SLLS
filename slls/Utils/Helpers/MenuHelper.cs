using System.Collections.Generic;
using System.Linq;
using slls.DAO;
using slls.Models;

namespace slls.Utils.Helpers
{
    public class MenuHelper
    {
        //public static List<Menu> OrdersSeeAlso(string ignore = "index")
        //{
        //    var db = new DbEntities();

        //    var seeAlso = from m in db.Menus.Where(m => m.Groups.Contains("ordersSeeAlso") && m.Action != ignore)
        //                  select m;
        //    return seeAlso.ToList();
        //}

        public static List<Menu> SeeAlso(string group = "", string ignoreAction = "", string ignoreController = "", string orderBy = "title")
        {
            var db = new DbEntities();

            //var menuItems = CacheProvider.GetAll<Menu>("menuitems");
            //var seeAlso = menuItems.Where(m => m.Groups.Contains(group) && (m.Action != ignoreAction || m.Controller != ignoreController));
            var seeAlso = db.Menus.Where(m => m.Groups.Contains(group) && (m.Action != ignoreAction || m.Controller != ignoreController));

            return orderBy == "title" ? seeAlso.OrderBy(m => m.Title).ToList() : seeAlso.OrderBy(m => m.SortOrder).ToList();
            
        }
    }
}