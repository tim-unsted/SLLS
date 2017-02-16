using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class ClassmarksController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        private readonly string _entityName = DbRes.T("Classmarks.Classmark", "FieldDisplayName");

        public ClassmarksController()
        {
            ViewBag.Title = DbRes.T("Classmarks", "EntityType");
        }

        // GET: Classmarks
        public ActionResult Index()
        {
            ViewData["SeeAlso"] = MenuHelper.SeeAlso("authlistsSeeAlso", ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["controller"].ToString());
            var classmarks = _db.Classmarks.Where(c => c.Deleted == false); //CacheProvider.GetAll<Classmark>("classmarks");
            return View(classmarks.OrderBy(x => x.ListPos).ToList());
        }
        
        // GET: Classmarks/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Add new " + _entityName;
            var viewModel = new ClassmarksAddViewModel();
            return PartialView(viewModel);
        }

        // POST: Classmarks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "Classmark,Code")] ClassmarksAddViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var newClassmark = new Classmark
                {
                    Classmark1 = viewModel.Classmark,
                    Code = viewModel.Code,
                    CanUpdate = true,
                    CanDelete = true,
                    InputDate = DateTime.Now
                };
                _db.Classmarks.Add(newClassmark);
                _db.SaveChanges();
                CacheProvider.RemoveCache("classmarks");
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }
            return PartialView(viewModel);
        }

        public ActionResult _add()
        {
            return PartialView();
        }

        // POST: Classmarks/_add
        [HttpPost]
        public JsonResult _add(Classmark classmark)
        {
            if (ModelState.IsValid)
            {
                var newClassmark = _db.Classmarks.FirstOrDefault(x => x.Classmark1 == classmark.Classmark1);

                if (newClassmark == null)
                {
                    newClassmark = new Classmark
                    {
                        Classmark1 = classmark.Classmark1,
                        CanUpdate = true,
                        CanDelete = true,
                        InputDate = DateTime.Now
                    };

                    _db.Classmarks.Add(newClassmark);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("classmarks");

                    return Json(new
                    {
                        success = true,
                        newData = newClassmark
                    });
                } else
                {
                    return Json(new
                    {
                        success = false,
                        errMsg = "This " + DbRes.T("Classmarks.Classmark", "FieldDisplayName") + " already exists!"
                    });
                }   
            }
            return null;
        }

        // GET: Classmarks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var classmark = _db.Classmarks.Find(id);
            if (classmark == null)
            {
                return HttpNotFound();
            }
            if (classmark.Deleted)
            {
                return HttpNotFound();
            }
            var cvm = new ClassmarksEditViewModel
            {
                Classmark = classmark.Classmark1,
                ClassmarkID = classmark.ClassmarkID,
                Code = classmark.Code,
                CanDelete = classmark.CanDelete,
                CanUpdate = classmark.CanUpdate
            };
            
            ViewBag.Title = "Edit " + _entityName;
            return PartialView(cvm);
        }

        // POST: Classmarks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "ClassmarkID,Classmark,Code")] ClassmarksEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return PartialView(viewModel);
            var classmark = _db.Classmarks.Find(viewModel.ClassmarkID);
            if (classmark == null)
            {
                return HttpNotFound();
            }
            if (classmark.Deleted)
            {
                return HttpNotFound();
            }
            //classmark.ClassmarkID = viewModel.ClassmarkID;
            classmark.Classmark1 = viewModel.Classmark;
            classmark.Code = viewModel.Code;
            classmark.LastModified = DateTime.Now;
            _db.Entry(classmark).State = EntityState.Modified;
            _db.SaveChanges();
            CacheProvider.RemoveCache("classmarks");
            return Json(new { success = true });
            //return RedirectToAction("Index");
        }


        public static int GetClassmarkId(string classmark)
        {
            classmark = classmark.Trim();
            var db = new DbEntities();
            var allClassmark = CacheProvider.GetAll<Classmark>("classmarks");
            var model = allClassmark.FirstOrDefault(x => String.Equals(x.Classmark1, classmark, StringComparison.OrdinalIgnoreCase));
            if (model != null) return model.ClassmarkID;
            //insert new classmark now ...
            var newClassmark = new Classmark
            {
                Classmark1 = classmark,
                CanUpdate = true,
                CanDelete = true,
                InputDate = DateTime.Now
            };
            db.Classmarks.Add(newClassmark);
            db.SaveChanges();
            CacheProvider.RemoveCache("classmarks");
            return newClassmark.ClassmarkID;
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            var cm = _db.Classmarks.Find(id);
            if (cm == null)
            {
                return HttpNotFound();
            }
            if (cm.Deleted)
            {
                return HttpNotFound();
            }
            if (cm.CanDelete == false)
            {
                return RedirectToAction("Index");
            }

            // Create new instance of DeleteConfirmationViewModel and pass it to _DeleteConfirmation Dialog (Reusable)
            DeleteConfirmationViewModel deleteConfirmationViewModel = new DeleteConfirmationViewModel
            {
                DeleteEntityId = id,
                HeaderText = _entityName,
                PostDeleteAction = "Delete",
                PostDeleteController = "Classmarks",
                DetailsText = cm.Classmark1
            };
            return PartialView("_DeleteConfirmation", deleteConfirmationViewModel);
        }

        [HttpPost]
        public ActionResult Delete(DeleteConfirmationViewModel dcvm)
        {
            // Get item which we need to delete from EntityFramework
            var item = _db.Classmarks.Find(dcvm.DeleteEntityId);

            if (item == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Classmarks.Remove(item);
                    _db.SaveChanges();
                    CacheProvider.RemoveCache("classmarks");
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