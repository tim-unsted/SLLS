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
        private readonly IEnumerable<Menu> _menuItems = CacheProvider.GetAll<Menu>("menuitems");

        public ActionResult TopMenu()
        {
            //Get the very top menu item ...
            var topLevelItem = _menuItems.FirstOrDefault(m => m.ParentID == null); // should be ID = -999
            var opacTopItem = _menuItems.FirstOrDefault(m => m.ParentID == topLevelItem.ID && m.Title == "OPAC");
            int parentMenuId = opacTopItem.ID;

            //Must get the entire menu at this stage ...
            var allItems = (from m in _menuItems.Where(m => m.IsVisible && m.IsEnabled).OrderBy(x => x.SortOrder) select m).ToList();
            
            //If the user is a Bailey Admin then grant them access to everything!
            if (User.IsInRole("Bailey Admin"))
            {
                var baileyAdminItems = from m in allItems
                                       where m.MenuArea == "OPAC"
                                       select m;

                ViewData["ParentMenuID"] = parentMenuId;
                return View(baileyAdminItems);
            }

            //Get only the menu items that match the user's roles
            //Note: GetRoles() returns the Role.Name, not the Role.ID as expected!
            var userRoles = Roles.GetUserRoles();

            var userItems = from m in allItems
                where m.MenuArea == "OPAC"
                      && m.Roles.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                          .Any(x => userRoles.Contains(x) || x == "All" || x == "Anonymous")
                select m;

            ViewData["ParentMenuID"] = parentMenuId;
            return View(userItems);
        }

        [AllowAnonymous]
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

            //Get just the menu items the admin user should have access to ...
            var userItems = GetAdminUserMenuItems(parentMenuId);

            ViewData["ParentMenuID"] = parentMenuId;
            return View(userItems);
        }

        public IEnumerable<Menu> GetAdminUserMenuItems(int parentMenuId = -40)
        {                        
            var allItems = (from m in _menuItems.Where(m => m.IsEnabled && m.IsVisible).OrderBy(x => x.SortOrder) select m).ToList();

            //If the user is a Bailey Admin then grant them access to everything!
            if (User.IsInRole("Bailey Admin"))
            {
                var baileyAdminItems = from m in allItems
                                       where m.MenuArea == "LibraryAdmin"
                                       select m;
                return baileyAdminItems;
            }
            else
            {
                //Get the admin menu items that match the user's roles
                //Note: GetRoles() returns the Role.Name, not the Role.ID as expected!
                var userRoles = Roles.GetUserRoles();
                var userItems = from m in allItems
                                where
                                    m.MenuArea == "LibraryAdmin" &&
                                    m.Roles.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                                        .Any(x => userRoles.Contains(x) || x == "All" || x == "Anonymous")
                                select m;
                return userItems;
            }
            
        }
    }
}