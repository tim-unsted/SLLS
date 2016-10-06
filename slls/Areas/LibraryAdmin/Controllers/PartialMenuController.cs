using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;

//***** LIBRARY ADMIN MENU  *****//

namespace slls.Areas.LibraryAdmin
{
    public class PartialMenuController : AdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        
        //Get the menu items from the database or cache ...
        //private readonly IEnumerable<Menu> menuItems = DAO.CacheProvider.GetMenusItems();
        private readonly IEnumerable<Menu> _menuItems = CacheProvider.GetAll<Menu>("menuitems");

        //Cache each admin user's menu for 2 hours
        //[OutputCache(Duration = 7200, VaryByParam = "*")]
        public ActionResult TopMenu()
        {
            
            
            //Get the very top term ...
            var topLevelItem = _menuItems.FirstOrDefault(m => m.ParentID == null); // should be ID = -999
            var opacTopItem = _menuItems.FirstOrDefault(m => m.ParentID == topLevelItem.ID && m.Title == "OPAC");
            int parentMenuId = opacTopItem.ID;

            var allItems = (from m in _menuItems.OrderBy(x => x.SortOrder) select m).ToList();

            //Get the current user's ID ...
            var id = User.Identity.GetUserId();

            //If the user is not logged in, only the menu items with the role "All" or "Anonymous"
            if (String.IsNullOrEmpty(id))
            {
                var someItems = from someitems in allItems.Where(y => y.Roles.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                            .Any(x => new[] { "All", "Anonymous" }.Contains(x)))
                                select someitems;
                ViewData["ParentMenuID"] = parentMenuId;
                return View(someItems.ToList());
            }
            //Get the menu items that match the user's roles
            //Note: GetRoles() returns the Role.Name, not the Role.ID as expected!
            var userRoles = Roles.GetUserRoles();

            var userItems = from m in allItems
                where m.MenuArea == "OPAC" 
                      && m.IsVisible
                      && m.Roles.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                          .Any(x => userRoles.Contains(x) || x == "All" || x == "Anonymous")
                select m;

            ViewData["ParentMenuID"] = parentMenuId;
            return View(userItems);
        }

        [AllowAnonymous]
        //Cache each admin user's menu for 2 hours
        //[OutputCache(Duration = 7200, VaryByParam = "*")]
        public ActionResult MainMenu()
        {
            var rd = ControllerContext.ParentActionViewContext.RouteData;
            var currentController = rd.GetRequiredString("controller");

            var searchWhere = new SelectList(new[] 
            {
                new { ID = "Titles", Name = "Titles" },
                new { ID = "Barcodes", Name = "Barcodes" },
                new { ID = "SupplierPeople", Name = "Contacts" },
                new { ID = "Suppliers", Name = "Suppliers" }
                
            }, "ID", "Name", currentController);
            ViewData["SearchWhere"] = searchWhere;

            //Get the very top term ...
            var topLevelItem = _menuItems.FirstOrDefault(m => m.ParentID == null); // should be ID = -999
            var adminTopItem = _menuItems.FirstOrDefault(m => m.ParentID == topLevelItem.ID && m.Title == "LibraryAdmin");
            int parentMenuId = adminTopItem.ID; // should be ID = -40

            var userItems = GetAdminUserMenuItems(parentMenuId);

            ViewData["ParentMenuID"] = parentMenuId;
            return View(userItems);
        }

        //Cache each admin user's menu for 2 hours
        //[OutputCache(Duration=7200, VaryByParam="none", Location=OutputCacheLocation.Client, NoStore=true)]
        public IEnumerable<Menu> GetAdminUserMenuItems(int parentMenuId = -40)
        {                        
            var allItems = (from m in _menuItems.OrderBy(x => x.SortOrder) select m).ToList();
            
            //Get the admin menu items that match the user's roles
            //Note: GetRoles() returns the Role.Name, not the Role.ID as expected!
            //var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var id = User.Identity.GetUserId();
            //var roles = userManager.GetRoles(id);
            var userRoles = Roles.GetUserRoles();
            var userItems = from m in allItems
                where
                    m.MenuArea == "LibraryAdmin" &&
                    m.Roles.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries)
                        .Any(x => userRoles.Contains(x) || x == "All" || x == "Anonymous")
                select m;
            return userItems;
        }
    }
}