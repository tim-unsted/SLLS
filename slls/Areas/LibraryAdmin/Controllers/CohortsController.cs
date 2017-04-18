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
    public class CohortsController : Controller
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/Cohorts
        public ActionResult Index()
        {
            var cohorts = _db.Cohorts.Where(x => x.CohortId != 1);
            ViewBag.Title = DbRes.T("Cohorts", "EntityType");
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(cohorts.ToList());
        }

        // GET: LibraryAdmin/Cohorts/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + DbRes.T("Cohorts.Cohort", "FieldDisplayName");
            var viewModel = new CohortsAddViewModel();
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Cohorts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Cohort")] CohortsAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var cohort = new Cohort()
                {
                    Cohort1 = viewModel.Cohort,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Today
                };
                _db.Cohorts.Add(cohort);
                _db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        // GET: LibraryAdmin/Cohorts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cohort = _db.Cohorts.Find(id);
            if (cohort == null)
            {
                return HttpNotFound();
            }
            var viewModel = new CohortsEditViewModel()
            {
                Cohort = cohort.Cohort1,
                CohortId = cohort.CohortId
            };
            ViewBag.Title = "Edit " + DbRes.T("Cohorts.Cohort", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Cohorts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CohortId,Cohort")] CohortsEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var cohort = _db.Cohorts.Find(viewModel.CohortId);
                if (cohort == null)
                {
                    return HttpNotFound();
                }
                cohort.Cohort1 = viewModel.Cohort;
                _db.Entry(cohort).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var cohort = _db.Cohorts.Find(id);
            if (cohort == null)
            {
                return HttpNotFound();
            }
            if (cohort.Deleted)
            {
                return HttpNotFound();
            }
            if (cohort.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel dcvm = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = DbRes.T("Cohorts.Cohort", "FieldDisplayName"),
                PostDeleteAction = "Delete",
                PostDeleteController = "Cohorts",
                DetailsText = cohort.Cohort1
            };
            return PartialView("_DeleteConfirmation", dcvm);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var cohort = _db.Cohorts.Find(dcvm.DeleteEntityId);

            if (cohort == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Cohorts.Remove(cohort);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("Cohorts");
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
