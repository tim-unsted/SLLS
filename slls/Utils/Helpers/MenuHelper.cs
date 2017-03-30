using System.Collections.Generic;
using System.Linq;
using slls.DAO;
using slls.Models;

namespace slls.Utils.Helpers
{
    public class MenuHelper
    {
        public static List<Menu> SeeAlso(string group = "", string ignoreAction = "", string ignoreController = "", string orderBy = "title")
        {
            var db = new DbEntities();
            IEnumerable<Menu> menuItems = CacheProvider.MenuItems();

            var seeAlso = menuItems.Where(m => m.Groups != null && m.Groups.Contains(group) && (m.Action != ignoreAction)); // || m.Controller != ignoreController));
            
            if (ignoreController != "")
            {
                seeAlso = seeAlso.Where(m => m.Controller != ignoreController);
            }

            return orderBy == "title" ? seeAlso.OrderBy(m => m.Title).ToList() : seeAlso.OrderBy(m => m.SortOrder).ToList();
            
        }
    }
}