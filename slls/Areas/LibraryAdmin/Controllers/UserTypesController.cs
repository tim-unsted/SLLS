using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class UserTypesController : Controller
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/UserTypes
        public ActionResult Index()
        {
            var userTypes = _db.UserTypes.Where(u => u.UserTypeId != 1).ToList();
            ViewBag.Title = DbRes.T("UserTypes", "EntityType");
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(userTypes);
        }

        // GET: LibraryAdmin/UserTypes/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + DbRes.T("UserTypes.User_Type", "FieldDisplayName");
            var viewModel = new UserTypesAddViewModel();
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/UserTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserType")] UserTypesAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userType = new UserType()
                {
                    UserType1 = viewModel.UserType,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Today
                };
                _db.UserTypes.Add(userType);
                _db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        // GET: LibraryAdmin/UserTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userType = _db.UserTypes.Find(id);
            if (userType == null)
            {
                return HttpNotFound();
            }
            var viewModel = new UserTypesEditViewModel()
            {
                UserType = userType.UserType1,
                UserTypeId = userType.UserTypeId
            };
            ViewBag.Title = "Edit " + DbRes.T("UserTypes.User_Type", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/UserTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserTypeId,UserType")] UserTypesEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userType = _db.UserTypes.Find(viewModel.UserTypeId);
                if (userType == null)
                {
                    return HttpNotFound();
                }
                userType.UserType1 = viewModel.UserType;
                _db.Entry(userType).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var userType = _db.UserTypes.Find(id);
            if (userType == null)
            {
                return HttpNotFound();
            }
            if (userType.Deleted)
            {
                return HttpNotFound();
            }
            if (userType.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel dcvm = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = DbRes.T("UserTypes.User_Type", "FieldDisplayName"),
                PostDeleteAction = "Delete",
                PostDeleteController = "UserTypes",
                DetailsText = userType.UserType1
            };
            return PartialView("_DeleteConfirmation", dcvm);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var userType = _db.UserTypes.Find(dcvm.DeleteEntityId);

            if (userType == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.UserTypes.Remove(userType);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("UserTypes");
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
            }
            base.Dispose(disposing);
        }
    }
}
