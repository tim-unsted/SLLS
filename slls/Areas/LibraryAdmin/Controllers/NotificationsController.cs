using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using slls.DAO;
using slls.Migrations;
using slls.Models;
using slls.Utils;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class NotificationsController : OpacAdminBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Notifications.Notification", "FieldDisplayName");
        private readonly GenericRepository _repository;

        public NotificationsController()
        {
            ViewBag.Title = DbRes.T("Notifications", "EntityType");
            _repository = new GenericRepository(typeof(Notification));
        }

        // Create a simple disctionary that we can used to fill a dropdown list in views
        public Dictionary<string, string> GetScopeTypes()
        {
            var scopeTypes = new Dictionary<string, string>();
            if (Roles.IsAdmin() || Roles.IsBaileyAdmin())
            {
                scopeTypes.Add("A", "Library Admin");
            }
            scopeTypes.Add("O", "OPAC");
            return scopeTypes;
        }

        // GET: LibraryAdmin/Notifications
        public ActionResult Index(string scope = "")
        {
            var allNotifications = from n in _db.Notifications select n;

            if (!string.IsNullOrEmpty(scope))
            {
                allNotifications = allNotifications.Where(n => n.Scope == scope);
            }

            var viewModel = new NotificationIndexViewModel()
            {
                Notifications = allNotifications,
                Scope = scope
            };

            ViewData["Scope"] = GetScopeTypes();
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("opacAdminSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(viewModel);
        }

        //Method used to supply the next empty position for a given scope
        public JsonResult GetNextPosition(string scope = "O")
        {
            var position = _db.Notifications.Where(n => n.Scope == scope).Select(n => n.Position).Max();

            return Json(new
            {
                success = true,
                Position = position + 1
            });
        }
        
        // GET: LibraryAdmin/Notifications/Create
        public ActionResult Create(string scope = "O")
        {
            ViewBag.Title = "Add New " + _entityName;
            ViewBag.ScopeTypes = GetScopeTypes();
            
            var position = _db.Notifications.Where(n => n.Scope == scope).Select(n => n.Position).Max();
            var viewModel = new NotificationsViewModel()
            {
                Scope = scope,
                Position = position + 1,
                Visible = true
            };
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Notifications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] 
        public ActionResult Create([Bind(Include = "Headline,Scope,Text,ExpireDate,Position,Visible")] NotificationsViewModel viewModel)
        {
            var position = 1;
            if (_db.Notifications.Any(n => n.Scope == viewModel.Scope))
            {
                position = _db.Notifications.Where(n => n.Scope == viewModel.Scope).Select(n => n.Position).Max();
            }
            //position = _db.Notifications.Where(n => n.Scope == viewModel.Scope).Select(n => n.Position).Max();
            var notification = new Notification()
            {
                Headline = viewModel.Headline,
                Text = viewModel.Text,
                Scope = viewModel.Scope,
                Position = viewModel.Position == 0 ? position : viewModel.Position,
                Visible = viewModel.Visible,
                ExpireDate = viewModel.ExpireDate,
                InputDate = DateTime.Now,
                InputUser = PublicFunctions.GetUserId() //User.Identity.GetUserId()
            };
            
            if (ModelState.IsValid)
            {
                _db.Notifications.Add(notification);
                _db.SaveChanges();
                CacheProvider.RemoveCache("notifications");
                return Json(new { success = true });
            }

            return View(viewModel);
        }

        // GET: LibraryAdmin/Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var notification = _db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }

            var viewModel = new NotificationsViewModel()
            {
                Headline = notification.Headline,
                Text = notification.Text,
                Scope = notification.Scope,
                Position = notification.Position,
                Visible = notification.Visible,
                ExpireDate = notification.ExpireDate,
                NotificationID = notification.NotificationID
            };

            ViewBag.Title = "Edit " + _entityName;
            ViewBag.ScopeTypes = GetScopeTypes();
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Notifications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] 
        public ActionResult Edit([Bind(Include = "NotificationID,Headline,Scope,Position,Visible,Text,ExpireDate")] NotificationsViewModel vm)
        {
            if (!ModelState.IsValid) return PartialView(vm);
            var notification = _db.Notifications.Find(vm.NotificationID);
            if (notification == null)
            {
                return HttpNotFound();
            }
            notification.Headline = vm.Headline;
            notification.Text = vm.Text;
            notification.ExpireDate = vm.ExpireDate;
            notification.Scope = vm.Scope;
            notification.Position = vm.Position;
            notification.Visible = vm.Visible;
            notification.LastModified = DateTime.Now;
            _db.Entry(notification).State = EntityState.Modified;
            _db.SaveChanges();
            CacheProvider.RemoveCache("notifications");
            return Json(new { success = true });
        }

        public ActionResult MoveNotificationUpList(int? id)
        {
            var notification = _db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }

            var itemAbove = (from n in _db.Notifications where n.Scope == notification.Scope && n.Position < notification.Position
                             orderby n.Position descending
                             select n).FirstOrDefault();
            var oldSortOrder = notification.Position;

            if (itemAbove != null)
            {
                var newSortOrder = itemAbove.Position;

                //Move the selected item up one place ...
                notification.Position = newSortOrder;
                _db.Entry(notification).State = EntityState.Modified;
                _db.SaveChanges();

                //Move the item just above the selected item down one place ...
                itemAbove.Position = oldSortOrder;
                _db.Entry(itemAbove).State = EntityState.Modified;
                _db.SaveChanges();

                CacheProvider.RemoveCache("notifications");
            }
            return RedirectToAction("Index", new { scope = notification.Scope });
        }

        public ActionResult MoveNotificationDownList(int id)
        {
            var notification = _db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }

            var oldSortOrder = notification.Position;

            var itemBelow = (from n in _db.Notifications where n.Scope == notification.Scope && n.Position > notification.Position
                             orderby n.Position ascending
                             select n).FirstOrDefault();

            if (itemBelow != null)
            {
                var newSortOrder = itemBelow.Position;

                //Move the selected item down one place ...
                notification.Position = newSortOrder;
                _db.Entry(notification).State = EntityState.Modified;
                _db.SaveChanges();

                //Move the item just below the selected item up one place ...
                itemBelow.Position = oldSortOrder;
                _db.Entry(itemBelow).State = EntityState.Modified;
                _db.SaveChanges();

                CacheProvider.RemoveCache("notifications");
            }

            return RedirectToAction("Index", new { scope = notification.Scope });
        }

        
        // GET: LibraryAdmin/Notifications/Delete/5
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var notification = _repository.GetById<Notification>(id);
            if (notification == null)
            {
                return HttpNotFound();
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "Notifications",
                DetailsText = notification.Headline
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var item = _db.Notifications.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Notifications.Remove(item);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("notifications");
                    return Json(new { success = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return PartialView("_DeleteConfirmation", dcvm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
                _repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
