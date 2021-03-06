﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;

namespace slls.Areas.Config
{
    public class MenuBuilderController : ConfigBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private ApplicationRoleManager _roleManager;

        public ApplicationRoleManager RoleManager
        {
            get { return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>(); }
            private set { _roleManager = value; }
        }

        // Create a set of simple dictionaries that we can used to fill dropdown lists in views ...
        public Dictionary<string, string> GetTargets()
        {
            return new Dictionary<string, string>
            {
                {"_self", "Opens in current page/tab"},
                {"_blank", "Opens in new page/tab"}
            };
        }

        public Dictionary<string, string> GetMenuAreas()
        {
            return new Dictionary<string, string>
            {
                {"", "None"},    
                {"LibraryAdmin", "Admin"},
                {"Config", "Config"},
                {"OPAC", "OPAC"}
            };
        }

        public Dictionary<string, string> GetMenuTypes()
        {
            return new Dictionary<string, string>
            {
                {"", "None"},    
                {"LibraryAdmin", "Admin"},
                {"Config", "Config"},
                {"OPAC", "OPAC"}
            };
        }

        public Dictionary<string, string> GetDataToggles()
        {
            return new Dictionary<string, string>
            {
                {"", ""},
                {"modal", "Modal Pop-Up"}
            };
        }

        public Dictionary<string, string> GetDataTargets()
        {
            return new Dictionary<string, string>
            {
                {"", ""},
                {"#stdModal", "Standard Pop-Up"},
                {"#lrgModal", "Large Pop-Up"}
            };
        }

        //Method used to supply a JSON list of parent menu itemss when selecting a menu area (Ajax stuf)
        public JsonResult GetParents(int AreaID = 0)
        {
            var parents = new SelectList(_db.Menus.Where(m => m.ParentID == AreaID).OrderBy(m => m.SortOrder).ToList(), "ID", "Title");

            return Json(new
            {
                success = true,
                ParentData = parents
            });
        }

        [AuthorizeRoles(Roles.BsAdmin)]
        public ActionResult LibraryAdmin()
        {
            return RedirectToAction("Index", new {id = "LibraryAdmin"});
        }
        
        [AuthorizeRoles(Roles.Administrator)]
        public ActionResult OPAC()
        {
            return RedirectToAction("Index", new { id = "OPAC" });
        }

        [AuthorizeRoles(Roles.BsAdmin)]
        public ActionResult Config()
        {
            return RedirectToAction("Index", new { id = "Config" });
        }

        
        [AuthorizeRoles(Roles.Administrator)]
        public ActionResult Index(string id = "OPAC")
        {
            var topLevelItem = _db.Menus.FirstOrDefault(m => m.ParentID == null); // should be ID = -999
            var areaTopItem = _db.Menus.FirstOrDefault(m => m.ParentID == topLevelItem.ID && m.Title == id);
            if (areaTopItem != null)
            {
                int parentMenuId = areaTopItem.ID;
            
                var allItems = (from m in _db.Menus where m.MenuArea == id orderby m.SortOrder select m).ToList();

                ViewData["MenuType"] = SelectListHelper.MenuList();
                ViewBag.Title = "Menu Editor - " + id;
                ViewBag.Menu = id;
                ViewData["ParentMenuID"] = parentMenuId;
                return View(allItems);
            }
            return null;
        }


        [HttpGet]
        public ActionResult Create(int? id)
        {
            var menuitem = _db.Menus.Find(id);

            //Establish what roles the current user has. Should be at least 'Admin' to get here ...
            var userRoles = Roles.GetUserRoles();

            //Get the list (string) of all roles the menu item has ...
            var allMenuRoles = menuitem.Roles;

            //Create a list of all roles the current use has access to ...
            var includedMenuRoles = new List<string> { "Admin", "All", "Anonymous", "Library Staff", "OPAC User" };

            //Create a list of menu areas the current use has access to ...
            var allowedAreas = new List<string> { "Menu", "", "OPAC", "MyLibrary", "Home" };

            //Add extra roles and menu area if allowed ...
            if (userRoles.Contains("Bailey Admin") || userRoles.Contains("Bailey Developer"))
            {
                includedMenuRoles.Add("Bailey Developer");
                includedMenuRoles.Add("Bailey Admin");
                allowedAreas.Add("LibraryAdmin");
                allowedAreas.Add("Config");
                allowedAreas.Add("BS");
            }

            int? parentId = menuitem.ParentID;
            int? grandParentId = null;
            int? greatGrandParentId = null;
            
            if (parentId != null)
            {
                var parentItem = _db.Menus.Find(parentId);
                grandParentId = parentItem.ParentID;
                if (grandParentId != null)
                {
                    var grandParentItem = _db.Menus.Find(grandParentId);
                    greatGrandParentId = grandParentItem.ParentID;
                }
            }

            //Only provide access to certain areas, depending on the user's roles ...
            ViewData["ParentArea"] = new SelectList(_db.Menus.Where(m => m.ParentID == grandParentId && allowedAreas.Contains(m.MenuArea) && m.MenuArea == menuitem.MenuArea).OrderBy(m => m.Title), "ID", "Title", parentId);
            ViewData["ParentID"] = new SelectList(_db.Menus.Where(m => m.ParentID == parentId && allowedAreas.Contains(m.MenuArea)).OrderBy(m => m.Title), "ID", "Title", menuitem.ParentID);

            //Create our view model of data to work with, based on the selected menu item ...
            var viewModel = new MenuViewModel()
            {
                ParentID = menuitem.ParentID,
                MenuArea = menuitem.MenuArea,
                LinkArea = menuitem.LinkArea,
                SortOrder = menuitem.SortOrder + 5,
                IsSelectable = true,
                IsEnabled = true,
                IsVisible = true,
                IsDivider = menuitem.Class == "divider",
                Target = "_self",
                Controller = menuitem.Controller,
                RolesList = RoleManager.Roles.Where(r => includedMenuRoles.Contains(r.Name)).ToList().Select(x => new SelectListItem
                {
                    Selected = allMenuRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            };

            //Get data for various drop-down list ...
            ViewBag.Targets = GetTargets();
            ViewBag.MenuAreas = GetMenuAreas();
            ViewBag.Controllers = MvcHelpers.GetControllerShortNames();
            ViewBag.DataToggles = GetDataToggles();
            ViewBag.DataTargets = GetDataTargets();

            ViewBag.Title = "Add New Menu Item";
            return PartialView(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MenuViewModel viewModel, params string[] selectedRole)
        {
            selectedRole = selectedRole ?? new string[] { };
            
            if (ModelState.IsValid)
            {
                var menuitem = new Menu
                {
                    Title = viewModel.Title,
                    ParentID = viewModel.ParentID,
                    MenuArea = viewModel.MenuArea,
                    Controller = viewModel.Controller,
                    Action = viewModel.Action,
                    LinkArea = viewModel.LinkArea,
                    URL = viewModel.Url,
                    ToolTip = viewModel.ToolTip,
                    DataToggle = viewModel.DataToggle,
                    DataTarget = viewModel.DataTarget,
                    Target = viewModel.Target,
                    IsSelectable = viewModel.IsSelectable,
                    IsEnabled = viewModel.IsEnabled,
                    IsVisible = viewModel.IsVisible,
                    Class = viewModel.IsDivider == true ? "divider" : null,
                    CanDelete = true,
                    Deleted = false,
                    SortOrder = viewModel.SortOrder,
                    Roles = string.Join(";",selectedRole)
                };

                _db.Menus.Add(menuitem);
                _db.SaveChanges();
                CacheProvider.RemoveCache("menuitems");

                return Json(new { success = true });
                //return RedirectToAction("Index", new { id = viewModel.MenuArea });
            }
            ViewBag.Title = "Add New Menu Item";
            return PartialView(viewModel);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var menuitem = _db.Menus.Find(id);
            if (menuitem == null)
            {
                return HttpNotFound();
            }

            //Establish what roles the current user has. Should be at least 'Admin' to get here ...
            var userRoles = Roles.GetUserRoles();

            //Get the list (string) of all roles the menu item has ...
            var allMenuRoles = menuitem.Roles;

            //Create a list of all roles the current use has access to ...
            var includedMenuRoles = new List<string> {"Admin", "All", "Anonymous", "Library Staff", "OPAC User"};

            //Create a list of menu areas the current use has access to ...
            var allowedAreas = new List<string> { "Menu", "", "OPAC", "MyLibrary", "Home" };
            
            //Add extra roles and menu area if allowed ...
            if (userRoles.Contains("Bailey Admin") || userRoles.Contains("Bailey Developer"))
            {
                includedMenuRoles.Add("Bailey Developer");
                includedMenuRoles.Add("Bailey Admin");
                allowedAreas.Add("LibraryAdmin");
                allowedAreas.Add("Config");
                allowedAreas.Add("BS");
            }

            //Create our view model of data to work with ...
            var viewModel = new MenuViewModel()
            {
                ID = menuitem.ID,
                ParentID = menuitem.ParentID,
                Title = menuitem.Title,
                MenuArea = menuitem.MenuArea,
                Controller = menuitem.Controller,
                Action = menuitem.Action,
                LinkArea = menuitem.LinkArea,
                Url = menuitem.URL,
                ToolTip = menuitem.ToolTip,
                Class = menuitem.Class,
                DataToggle = menuitem.DataToggle,
                DataTarget = menuitem.DataTarget,
                Target = menuitem.Target,
                IsSelectable = menuitem.IsSelectable,
                IsEnabled = menuitem.IsEnabled,
                IsVisible = menuitem.IsVisible,
                IsDivider = menuitem.Class == "divider",
                SortOrder = menuitem.SortOrder,
                Roles = menuitem.Roles,
                Description = menuitem.Description,
                RolesList = RoleManager.Roles.Where(r => includedMenuRoles.Contains(r.Name)).ToList().Select(x => new SelectListItem
                {
                    Selected = allMenuRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            };
            
            //Get data for various drop-down lists ...
            ViewBag.Targets = GetTargets();
            ViewBag.MenuAreas = GetMenuAreas();
            ViewBag.Controllers = MvcHelpers.GetControllerShortNames();
            ViewBag.DataToggles = GetDataToggles();
            ViewBag.DataTargets = GetDataTargets();

            //Establish the parent, grandparent and greatgrandparent (if applicable) of the selected menu item ...
            int? parentId = null;
            int? grandParentId = null;
            int? greatGrandParentId = null;

            parentId = menuitem.ParentID;
            if (parentId != null)
            {
                var parentItem = _db.Menus.Find(parentId);
                grandParentId = parentItem.ParentID;
                if(grandParentId != null)
                {
                    var grandParentItem = _db.Menus.Find(grandParentId);
                    greatGrandParentId = grandParentItem.ParentID;
                }
            }

            //Only provide access to certain areas, depending on the user's roles ...
            ViewData["ParentArea"] = new SelectList(_db.Menus.Where(m => m.ParentID == greatGrandParentId && allowedAreas.Contains(m.MenuArea) && m.MenuArea == menuitem.MenuArea).OrderBy(m => m.Title), "ID", "Title", grandParentId);
            ViewData["ParentID"] = new SelectList(_db.Menus.Where(m => m.ParentID == grandParentId && allowedAreas.Contains(m.MenuArea)).OrderBy(m => m.Title), "ID", "Title", menuitem.ParentID);
            
            ViewBag.Title = "Edit Menu Item";
            return PartialView(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MenuViewModel viewModel, params string[] selectedRole)
        {
            var menuitem = _db.Menus.Find(viewModel.ID);

            if (menuitem == null)
            {
                return HttpNotFound();
            }

            selectedRole = selectedRole ?? new string[] { };

            if (ModelState.IsValid)
            {
                menuitem.Title = viewModel.Title;
                menuitem.ParentID = viewModel.ParentID;
                menuitem.MenuArea = viewModel.MenuArea;
                menuitem.Controller = viewModel.Controller;
                menuitem.Action = viewModel.Action;
                menuitem.LinkArea = viewModel.LinkArea;
                menuitem.URL = viewModel.Url;
                menuitem.ToolTip = viewModel.ToolTip;
                menuitem.Class = viewModel.Class;
                menuitem.DataToggle = viewModel.DataToggle;
                menuitem.DataTarget = viewModel.DataTarget;
                menuitem.Target = viewModel.Target;
                menuitem.IsSelectable = viewModel.IsSelectable;
                menuitem.IsEnabled = viewModel.IsEnabled;
                menuitem.IsVisible = viewModel.IsVisible;
                menuitem.Class = viewModel.IsDivider == true ? "divider" : null;
                menuitem.SortOrder = viewModel.SortOrder;
                menuitem.Roles = string.Join(";",selectedRole);

                _db.Entry(menuitem).State = EntityState.Modified;
                _db.SaveChanges();
                CacheProvider.RemoveCache("menuitems");

                //return RedirectToAction("Index", new { id = viewModel.MenuArea });
                return Json(new { success = true });
            }
            ViewBag.Title = "Edit Menu Item";
            return PartialView(viewModel);
        }
        

        public PartialViewResult Details(int id)
        {
            var menuitem = _db.Menus.Find(id);
            if (menuitem == null)
            {
                return null;
            }

            var viewModel = new MenuViewModel()
            {
                ID = menuitem.ID,
                ParentID = menuitem.ParentID ?? 0,
                Title = menuitem.Title,
                MenuArea = menuitem.MenuArea,
                Controller = menuitem.Controller,
                Action = menuitem.Action,
                LinkArea = menuitem.LinkArea,
                Url = menuitem.URL,
                ToolTip = menuitem.ToolTip,
                Class = menuitem.Class,
                IsDivider = menuitem.Class == "divider",
                DataToggle = menuitem.DataToggle,
                DataTarget = menuitem.DataTarget,
                Target = menuitem.Target,
                IsSelectable = menuitem.IsSelectable,
                IsEnabled = menuitem.IsEnabled,
                IsVisible = menuitem.IsVisible,
                SortOrder = menuitem.SortOrder
            };

            viewModel.Parent = (viewModel.ParentID == 0) ? "" : _db.Menus.Find(menuitem.ParentID).Title;

            return this.PartialView("_Details", viewModel);
        }


        public ActionResult MoveUp(int id)
        {
            var menuItem = _db.Menus.Find(id);
            if(menuItem == null)
            {
                return HttpNotFound();
            }

            var itemAbove = (from m in _db.Menus
                where m.ParentID == menuItem.ParentID && m.SortOrder < menuItem.SortOrder
                orderby m.SortOrder descending
                select m).FirstOrDefault();
            var oldSortOrder = menuItem.SortOrder;

            if (itemAbove != null)
            {
                var newSortOrder = itemAbove.SortOrder ?? 0;

                //Move the selected item up one place ...
                menuItem.SortOrder = newSortOrder;
                _db.Entry(menuItem).State = EntityState.Modified;
                _db.SaveChanges();

                //Move the item just above the selected item down one place ...
                itemAbove.SortOrder = oldSortOrder;
                _db.Entry(itemAbove).State = EntityState.Modified;
                _db.SaveChanges();

                CacheProvider.RemoveCache("menuitems");
            }

            return RedirectToAction("Index", new{id = menuItem.MenuArea});

        }


        public ActionResult MoveDown(int id)
        {
            var menuItem = _db.Menus.Find(id);
            if(menuItem == null)
            {
                return HttpNotFound();
            }

            var oldSortOrder = menuItem.SortOrder;

            var itemBelow = (from m in _db.Menus
                where m.ParentID == menuItem.ParentID && m.SortOrder > menuItem.SortOrder
                orderby m.SortOrder ascending
                select m).FirstOrDefault();
            
            if (itemBelow != null)
            {
                var newSortOrder = itemBelow.SortOrder ?? 0;

                //Move the selected item down one place ...
                menuItem.SortOrder = newSortOrder;
                _db.Entry(menuItem).State = EntityState.Modified;
                _db.SaveChanges();

                //Move the item just below the selected item up one place ...
                itemBelow.SortOrder = oldSortOrder;
                _db.Entry(itemBelow).State = EntityState.Modified;
                _db.SaveChanges();

                CacheProvider.RemoveCache("menuitems");
            }

            return RedirectToAction("Index", new{id = menuItem.MenuArea});
        }


        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var menuitem = _db.Menus.Find(id);
            if (menuitem == null)
            {
                return HttpNotFound();
            }

            var menuArea = menuitem.MenuArea;
            
            //Check if we can delete this item ...
            if (menuitem.CanDelete == false)
            {
                //return RedirectToAction("Index", new { id = menuArea });
                return null; //Json(new { success = true });
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = "Menu Item",
                PostDeleteAction = "Delete",
                PostDeleteController = "MenuBuilder",
                DetailsText = menuitem.Title ?? "" + menuitem.Class ?? ""
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.Menus.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Menus.Remove(item);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("menuitems");
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return PartialView("_DeleteConfirmation", dcvm);
        }
    }
}