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
    public class GendersController : Controller
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/Genders
        public ActionResult Index()
        {
            ViewBag.Title = DbRes.T("Genders", "EntityType");
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(_db.Genders.ToList());
        }

        // GET: LibraryAdmin/Genders/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + DbRes.T("Genders.Gender", "FieldDisplayName");
            var viewModel = new GendersAddViewModel();
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Genders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Gender")] GendersAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var gender = new Gender()
                {
                    Gender1 = viewModel.Gender,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Today
                };
                _db.Genders.Add(gender);
                _db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        // GET: LibraryAdmin/Genders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var gender = _db.Genders.Find(id);
            if (gender == null)
            {
                return HttpNotFound();
            }
            var viewModel = new GendersEditViewModel()
            {
                Gender = gender.Gender1,
                GenderId = gender.GenderId
            };
            ViewBag.Title = "Edit " + DbRes.T("Genders.Gender", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Genders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GenderId,Gender")] GendersEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var gender = _db.Genders.Find(viewModel.GenderId);
                if (gender == null)
                {
                    return HttpNotFound();
                }
                gender.Gender1 = viewModel.Gender;
                _db.Entry(gender).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var gender = _db.Genders.Find(id);
            if (gender == null)
            {
                return HttpNotFound();
            }
            if (gender.Deleted)
            {
                return HttpNotFound();
            }
            if (gender.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel dcvm = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = DbRes.T("Genders.Gender", "FieldDisplayName"),
                PostDeleteAction = "Delete",
                PostDeleteController = "Genders",
                DetailsText = gender.Gender1
            };
            return PartialView("_DeleteConfirmation", dcvm);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var gender = _db.Genders.Find(dcvm.DeleteEntityId);

            if (gender == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Genders.Remove(gender);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("Genders");
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
