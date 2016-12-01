using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;

//***** CONFIG MENU  *****//

namespace slls.Areas.Config
{
    public class PartialMenuController : ConfigBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _customerPackage = App_Settings.GlobalVariables.Package;

        //Get the menu items from the database or cache ...
        private readonly IEnumerable<Menu> _menuItems = CacheProvider.MenuItems();

        [AllowAnonymous]
        public ActionResult TopMenu()
        {
            //Get the very top menu item ...
            var topLevelItem = _menuItems.FirstOrDefault(m => m.ParentID == null); // should be ID = -999
            var opacTopItem = _menuItems.FirstOrDefault(m => m.ParentID == topLevelItem.ID && m.Title == "OPAC");
            int parentMenuId = opacTopItem.ID;

            var allItems = (from m in _menuItems.Where(m => m.MenuArea == "OPAC" && m.IsVisible && m.Packages != null && m.Packages.Contains(_customerPackage)).OrderBy(x => x.SortOrder) select m).ToList();
            
            //If the user is a Bailey Admin then grant them access to everything!
            if (User.IsInRole("Bailey Admin"))
            {
                var baileyAdminItems = from m in allItems
                    select m;

                ViewData["ParentMenuID"] = parentMenuId;
                return View(baileyAdminItems);
            }
            //Get the menu items that match the user's roles and package ...
            //Note: GetRoles() returns the Role.Name, not the Role.ID as expected!
            var userRoles = Roles.GetUserRoles();

            var userItems = from m in allItems
                            where 
                                m.Roles.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Any(x => userRoles.Contains(x))
                            select m;

            ViewData["ParentMenuID"] = parentMenuId;
            return View(userItems);
        }

        [AllowAnonymous]
        public ActionResult MainMenu()
        {
            //Get the very top term ...
            var topLevelItem = _menuItems.FirstOrDefault(m => m.ParentID == null); // should be ID = -999
            var configTopItem = _menuItems.FirstOrDefault(m => m.ParentID == topLevelItem.ID && m.Title == "Config");
            int parentMenuId = configTopItem.ID;

            var allItems = (from m in _menuItems.Where(m => m.MenuArea == "Config" && m.IsVisible && m.Packages != null && m.Packages.Contains(_customerPackage)).OrderBy(x => x.SortOrder) select m).ToList();

            //If the user is a Bailey Admin then grant them access to everything!
            if (User.IsInRole("Bailey Admin"))
            {
                var baileyAdminItems = from m in allItems
                                       select m;
                ViewData["ParentMenuID"] = parentMenuId;
                return View(baileyAdminItems);
            }
            // Get the config menu items that match the user's roles ...
            // To access this menu, the user must already be a 'System Admin' (or 'Bailey Admin'), so that's the only role to look for
            var userItems = from m in allItems
                            where 
                                m.Roles.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Any(role => role == "System Admin")
                            select m;

            ViewData["ParentMenuID"] = parentMenuId;
            return View(userItems);
        }
    }
}