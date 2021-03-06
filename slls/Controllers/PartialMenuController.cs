﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;

namespace slls.Controllers
{
    public class PartialMenuController : sllsBaseController
    {
        private readonly DbEntities _db = new DbEntities();

        public PartialMenuController()
        {
        }

        [AllowAnonymous]
        public ActionResult MainMenu()
        {
            //Home page does not need a sub menu ...
            return null;
        }

        [AllowAnonymous]
        public ActionResult TopMenu()
        {
            var _menuItems = CacheProvider.GetAll<Menu>("menuitems").ToList();
            //Get the very top term ...
            var topLevelItem = _menuItems.FirstOrDefault(m => m.ParentID == null); // should be ID = -999
            var opacTopItem = _menuItems.FirstOrDefault(m => m.ParentID == topLevelItem.ID && m.Title == "OPAC");
            int parentMenuId = opacTopItem.ID;

            var allItems = (from m in _menuItems.Where(m => m.IsVisible).OrderBy(x => x.SortOrder) select m).ToList();

            //Get the current user's ID ...
            var id = Utils.PublicFunctions.GetUserId(); //Utils.PublicFunctions.GetUserId(); //User.Identity.GetUserId();

            //If the user is not logged in, only the menu items with the role "All" or "Anonymous"
            if (String.IsNullOrEmpty(id))
            {
                var someItems = from someitems in allItems.Where(y => y.Roles.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                            .Any(x => new String[] { "All", "Anonymous" }.Contains(x)))
                                select someitems;
                ViewData["ParentMenuID"] = parentMenuId;
                return View(someItems.ToList());
            }

            //If the user is a Bailey Admin then grant them access to everything!
            if (User.IsInRole("Bailey Admin"))
            {
                var baileyAdminItems = from m in allItems
                                       where m.MenuArea == "OPAC"
                                       select m;

                ViewData["ParentMenuID"] = parentMenuId;
                return View(baileyAdminItems);
            }
            else
            {
                //Get the menu items that match the user's roles
                //Note: GetRoles() returns the Role.Name, not the Role.ID as expected!
                var userRoles = Roles.GetUserRoles();
                //var menuAreas = "OPAC";
                var userItems = from m in allItems
                                //where menuAreas.Contains(m.MenuArea) 
                                where 
                                m.MenuArea == "OPAC"
                                && m.IsVisible
                                && m.Roles.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                                    .Any(x => userRoles.Contains(x) || x == "All" || x == "Anonymous")
                                select m;

                ViewData["ParentMenuID"] = parentMenuId;
                return View(userItems);

            }
        }
    }
}