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
    public class ClassesController : Controller
    {
        private readonly DbEntities _db = new DbEntities();

        // GET: LibraryAdmin/Classes
        public ActionResult Index()
        {
            var classes = _db.Classes.Where(x => x.ClassId != 1);
            ViewBag.Title = DbRes.T("Classes", "EntityType");
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            return View(classes.ToList());
        }

        // GET: LibraryAdmin/Classes/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add New " + DbRes.T("Classes.Class", "FieldDisplayName");
            var viewModel = new ClassesAddViewModel();
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Classes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Class")] ClassesAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var Class = new Class()
                {
                    Class1 = viewModel.Class,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Today
                };
                _db.Classes.Add(Class);
                _db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        // GET: LibraryAdmin/Classes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Class = _db.Classes.Find(id);
            if (Class == null)
            {
                return HttpNotFound();
            }
            var viewModel = new ClassesEditViewModel()
            {
                Class = Class.Class1,
                ClassId = Class.ClassId
            };
            ViewBag.Title = "Edit " + DbRes.T("Classes.Class", "FieldDisplayName");
            return PartialView(viewModel);
        }

        // POST: LibraryAdmin/Classes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClassId,Class")] ClassesEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var Class = _db.Classes.Find(viewModel.ClassId);
                if (Class == null)
                {
                    return HttpNotFound();
                }
                Class.Class1 = viewModel.Class;
                _db.Entry(Class).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var Class = _db.Classes.Find(id);
            if (Class == null)
            {
                return HttpNotFound();
            }
            if (Class.Deleted)
            {
                return HttpNotFound();
            }
            if (Class.CanDelete == false)
            {
                return RedirectToAction("Index");
            }
            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel dcvm = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = DbRes.T("Classes.Class", "FieldDisplayName"),
                PostDeleteAction = "Delete",
                PostDeleteController = "Classes",
                DetailsText = Class.Class1
            };
            return PartialView("_DeleteConfirmation", dcvm);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework 
            var Class = _db.Classes.Find(dcvm.DeleteEntityId);

            if (Class == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Classes.Remove(Class);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("Classes");
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
