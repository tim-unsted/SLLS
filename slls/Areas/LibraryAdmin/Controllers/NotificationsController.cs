using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class NotificationsController : Controller
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
            return new Dictionary<string, string>
            {
                {"A", "Library Admin"},
                {"O", "OPAC"},
                {"C", "Config"}
            };
        }

        // GET: LibraryAdmin/Notifications
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("opacAdminSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_db.Notifications.ToList());
        }

        
        // GET: LibraryAdmin/Notifications/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + _entityName;
            ViewBag.ScopeTypes = GetScopeTypes();
            var viewModel = new NotificationsViewModel()
            {
                Scope = "O"
            };
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Notifications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] 
        public ActionResult Create([Bind(Include = "Headline,Scope,Text,ExpireDate")] NotificationsViewModel viewModel)
        {
            var notification = new Notification()
            {
                Headline = viewModel.Headline,
                Text = viewModel.Text,
                Scope = viewModel.Scope,
                ExpireDate = viewModel.ExpireDate,
                InputDate = DateTime.Now,
                InputUser = User.Identity.GetUserId()
            };
            
            if (ModelState.IsValid)
            {
                _db.Notifications.Add(notification);
                _db.SaveChanges();
                return Json(new { success = true });
                //return RedirectToAction("Index");
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

            Notification notification = _db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }

            var viewModel = new NotificationsViewModel()
            {
                Headline = notification.Headline,
                Text = notification.Text,
                Scope = notification.Scope,
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
        public ActionResult Edit([Bind(Include = "NotificationID,Headline,Scope,Text,ExpireDate")] NotificationsViewModel vm)
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
            notification.LastModified = DateTime.Now;
            _db.Entry(notification).State = EntityState.Modified;
            _db.SaveChanges();
            return Json(new { success = true });
            //return RedirectToAction("Index");
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
